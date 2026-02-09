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

    private readonly Dictionary<Vector2Int, PropRandom> _spawnedChunks = new Dictionary<Vector2Int, PropRandom>();

    private Vector3 _lastCameraPosition;
    private float _cullDistanceSqr;

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

    private void RecalculateCullDistance()
    {
        // Rough but stable: based on camera world size + chunk size.
        Vector2 minPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.min);
        Vector2 maxPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.max);
        Vector2 viewSize = maxPoint - minPoint;

        _cullDistanceSqr = Mathf.Max(viewSize.sqrMagnitude, chunkSize.sqrMagnitude) * 3f;
    }

    private void SpawnChunksAroundCamera(bool force)
    {
        Vector2 camPos = referenceCamera.transform.position;

        // Determine how many chunks are needed to cover the viewport + 1 border.
        Vector2 minPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.min);
        Vector2 maxPoint = referenceCamera.ViewportToWorldPoint(referenceCamera.rect.max);

        Vector2Int minCoord = WorldToChunkCoord(minPoint) + new Vector2Int(-1, -1);
        Vector2Int maxCoord = WorldToChunkCoord(maxPoint) + new Vector2Int(1, 1);

        for (int y = minCoord.y; y <= maxCoord.y; y++)
        {
            for (int x = minCoord.x; x <= maxCoord.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);

                if (!force && _spawnedChunks.ContainsKey(coord))
                    continue;

                if (_spawnedChunks.ContainsKey(coord))
                    continue;

                Vector3 worldPos = ChunkCoordToWorld(coord);
                PropRandom chunk = SpawnChunk(worldPos);
                if (chunk != null)
                    _spawnedChunks.Add(coord, chunk);
            }
        }
    }

    private Vector2Int WorldToChunkCoord(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / chunkSize.x);
        int y = Mathf.RoundToInt(worldPos.y / chunkSize.y);
        return new Vector2Int(x, y);
    }

    private Vector3 ChunkCoordToWorld(Vector2Int coord)
    {
        return new Vector3(coord.x * chunkSize.x, coord.y * chunkSize.y, transform.position.z);
    }

    private PropRandom SpawnChunk(Vector3 spawnPosition, int variant = -1)
    {
        if (terrainChunks == null || terrainChunks.Length < 1)
            return null;

        int rand = variant < 0 ? Random.Range(0, terrainChunks.Length) : variant;
        PropRandom chunk = Instantiate(terrainChunks[rand], transform);
        chunk.transform.position = spawnPosition;
        return chunk;
    }

    private void CullChunks()
    {
        Vector3 camPos = referenceCamera.transform.position;

        // Iterate dictionary values (no need to rely on transform children)
        // If you want deletion, collect keys to remove.
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