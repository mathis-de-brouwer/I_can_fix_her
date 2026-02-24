using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Camera referenceCamera;
    public float checkInterval = 0.5f;

    [Header("Chunk Settings")]
    public PropRandom[] terrainChunks;
    public Vector2 chunkSize = new Vector2(20f, 20f);
    public bool deleteCulledChunks = false;

    [Header("Chunk Groups")]
    [Tooltip("Multi-chunk structures that can appear on the map.")]
    public ChunkGroupDefinition[] chunkGroups;

    [Tooltip("0-1 chance that any given free coord tries to place a group instead of a random chunk.")]
    [Range(0f, 1f)]
    public float groupSpawnChance = 0.15f;

    // Internal

    private readonly Dictionary<Vector2Int, PropRandom> _spawnedChunks = new Dictionary<Vector2Int, PropRandom>();

    private Vector3 _lastCameraPosition;
    private float _cullDistanceSqr;

    private readonly Dictionary<ChunkGroupDefinition, int> _groupInstanceCount = new Dictionary<ChunkGroupDefinition, int>();
    private readonly HashSet<string> _activeFlags = new HashSet<string>();

    // Public API

    /// <summary>Unlock a flag so flag-gated chunk groups become eligible.</summary>
    public void SetFlag(string flag) => _activeFlags.Add(flag);

    /// <summary>Remove a flag.</summary>
    public void ClearFlag(string flag) => _activeFlags.Remove(flag);

    // Unity

    private void Start()
    {
        if (!referenceCamera)
            Debug.LogError("MapController cannot work without a reference camera.");

        if (terrainChunks == null || terrainChunks.Length < 1)
            Debug.LogError("There are no Terrain Chunks assigned, so the map cannot be dynamically generated.");

        RecalculateCullDistance();
        SpawnChunksAroundCamera(force: true);
        StartCoroutine(HandleMapCheck());
    }

    private void Reset()
    {
        referenceCamera = Camera.main;
    }

    // Map check coroutine

    private IEnumerator HandleMapCheck()
    {
        for (;;)
        {
            yield return new WaitForSeconds(checkInterval);

            Vector3 moveDelta = referenceCamera.transform.position - _lastCameraPosition;
            if (moveDelta.sqrMagnitude <= 0.01f)
                continue;

            RecalculateCullDistance();
            CullChunks();
            SpawnChunksAroundCamera(force: false);

            _lastCameraPosition = referenceCamera.transform.position;
        }
    }

    // Cull distance

    private void RecalculateCullDistance()
    {
        Vector2 minPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.min);
        Vector2 maxPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.max);
        Vector2 viewSize = maxPoint - minPoint;

        _cullDistanceSqr = Mathf.Max(viewSize.sqrMagnitude, chunkSize.sqrMagnitude) * 3f;
    }

    // Spawning

    private void SpawnChunksAroundCamera(bool force)
    {
        Vector2 minPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.min);
        Vector2 maxPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.max);

        Vector2Int minCoord = WorldToChunkCoord(minPoint) + new Vector2Int(-1, -1);
        Vector2Int maxCoord = WorldToChunkCoord(maxPoint) + new Vector2Int(1, 1);

        for (int y = minCoord.y; y <= maxCoord.y; y++)
        {
            for (int x = minCoord.x; x <= maxCoord.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);

                if (_spawnedChunks.ContainsKey(coord))
                    continue;

                // Try a group first; fall back to a single chunk.
                if (!TrySpawnGroupAt(coord))
                {
                    PropRandom chunk = SpawnChunk(ChunkCoordToWorld(coord));
                    if (chunk != null)
                        _spawnedChunks.Add(coord, chunk);
                }
            }
        }
    }

    // Chunk groups

    private bool TrySpawnGroupAt(Vector2Int anchorCoord)
    {
        if (chunkGroups == null || chunkGroups.Length == 0)
            return false;

        if (Random.value > groupSpawnChance)
            return false;

        List<ChunkGroupDefinition> eligible = new List<ChunkGroupDefinition>();
        foreach (ChunkGroupDefinition group in chunkGroups)
        {
            if (!IsGroupEligible(group))
                continue;

            if (!GroupFitsAt(group, anchorCoord))
                continue;

            eligible.Add(group);
        }

        if (eligible.Count == 0)
            return false;

        ChunkGroupDefinition chosen = eligible[Random.Range(0, eligible.Count)];
        SpawnGroup(chosen, anchorCoord);
        return true;
    }

    private bool IsGroupEligible(ChunkGroupDefinition group)
    {
        if (group == null || group.entries == null || group.entries.Length == 0)
            return false;

        if (!string.IsNullOrEmpty(group.requiredFlag) && !_activeFlags.Contains(group.requiredFlag))
            return false;

        if (group.maxInstances >= 0)
        {
            _groupInstanceCount.TryGetValue(group, out int count);
            if (count >= group.maxInstances)
                return false;
        }

        return true;
    }

    private bool GroupFitsAt(ChunkGroupDefinition group, Vector2Int anchor)
    {
        foreach (ChunkGroupDefinition.ChunkEntry entry in group.entries)
        {
            if (_spawnedChunks.ContainsKey(anchor + entry.gridOffset))
                return false;
        }
        return true;
    }

    private void SpawnGroup(ChunkGroupDefinition group, Vector2Int anchor)
    {
        foreach (ChunkGroupDefinition.ChunkEntry entry in group.entries)
        {
            Vector2Int coord = anchor + entry.gridOffset;
            PropRandom chunk = Instantiate(entry.prefab, transform);
            chunk.transform.position = ChunkCoordToWorld(coord);
            _spawnedChunks.Add(coord, chunk);
        }

        if (!_groupInstanceCount.ContainsKey(group))
            _groupInstanceCount[group] = 0;
        _groupInstanceCount[group]++;
    }

    // Helpers

    private PropRandom SpawnChunk(Vector3 worldPos)
    {
        if (terrainChunks == null || terrainChunks.Length < 1)
            return null;

        PropRandom chunk = Instantiate(terrainChunks[Random.Range(0, terrainChunks.Length)], transform);
        chunk.transform.position = worldPos;
        return chunk;
    }

    private Vector2Int WorldToChunkCoord(Vector2 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / chunkSize.x),
            Mathf.FloorToInt(worldPos.y / chunkSize.y));
    }

    private Vector2Int WorldToChunkCoord(Vector3 worldPos) =>
        WorldToChunkCoord(new Vector2(worldPos.x, worldPos.y));

    private Vector3 ChunkCoordToWorld(Vector2Int coord)
    {
        return new Vector3(coord.x * chunkSize.x, coord.y * chunkSize.y, transform.position.z);
    }

    // Culling

    private void CullChunks()
    {
        Vector3 camPos = referenceCamera.transform.position;
        List<Vector2Int> toRemove = deleteCulledChunks ? new List<Vector2Int>() : null;

        foreach (KeyValuePair<Vector2Int, PropRandom> kvp in _spawnedChunks)
        {
            PropRandom chunk = kvp.Value;
            if (chunk == null)
                continue;

            Vector2 dist = camPos - chunk.transform.position;
            bool cull = dist.sqrMagnitude > _cullDistanceSqr;

            chunk.gameObject.SetActive(!cull);

            if (deleteCulledChunks && cull)
            {
                Destroy(chunk.gameObject);
                toRemove.Add(kvp.Key);
            }
        }

        if (toRemove != null)
        {
            for (int i = 0; i < toRemove.Count; i++)
                _spawnedChunks.Remove(toRemove[i]);
        }
    }
}