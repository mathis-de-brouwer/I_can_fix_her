using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum P2SpawnDirection { Left, Right, Up, Down }

[CreateAssetMenu(menuName = "P2/Card Effects/Spawn Wave Directional")]
public sealed class P2SpawnWaveDirectionalCardEffect : P2CardEffect
{
    [Header("Direction")]
    [SerializeField] private P2SpawnDirection spawnDirection = P2SpawnDirection.Left;

    [Header("Wave")]
    [SerializeField] private int waveSize = 6;
    [SerializeField] private float spawnInterval = 0.3f;

    [Header("Placement")]
    [Tooltip("Distance from the player position along the spawn direction.")]
    [SerializeField] private float edgeOffset = 8f;

    [Tooltip("Random spread perpendicular to the spawn direction.")]
    [SerializeField] private float spread = 3f;

    [Header("Spawn table")]
    [Tooltip("Each entry is a prefab + weight. One entry = always that type. " +
             "Multiple entries = weighted random pick per spawn. Falls back to card.prefab if empty.")]
    [SerializeField] private List<P2SpawnEntry> spawnTable = new List<P2SpawnEntry>();

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (context == null || context.coroutineRunner == null)
        {
            Debug.LogWarning($"{nameof(P2SpawnWaveDirectionalCardEffect)}: no coroutine runner.");
            return;
        }

        if (!SpawnTable.IsValid(spawnTable) && (card == null || card.prefab == null))
        {
            Debug.LogWarning($"{nameof(P2SpawnWaveDirectionalCardEffect)}: no prefabs to spawn.");
            return;
        }

        context.coroutineRunner.StartCoroutine(SpawnRoutine(card, context));
    }

    private IEnumerator SpawnRoutine(P2Card card, P2CardEffectContext context)
    {
        Vector3 origin = context.target != null ? context.target.position
                       : context.spawnPoint != null ? context.spawnPoint.position
                       : Vector3.zero;

        Vector3 dir = DirectionToVector(spawnDirection);
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f);

        float magnitudeMult = 1f;
        if (card != null && card.scaleMagnitudeWithTime && context.timeScaling != null)
            magnitudeMult = context.timeScaling.GetMagnitudeMultiplier(context.elapsedSeconds);

        int count = Mathf.Max(1, Mathf.CeilToInt(waveSize * magnitudeMult));
        Debug.Log($"{nameof(P2SpawnWaveDirectionalCardEffect)}: Spawning {count} from {spawnDirection}.");

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = SpawnTable.IsValid(spawnTable)
                ? SpawnTable.Pick(spawnTable)
                : card?.prefab;

            if (prefab != null)
            {
                Vector3 pos = origin + dir * edgeOffset + perp * Random.Range(-spread, spread);
                Instantiate(prefab, pos, Quaternion.identity);
            }

            if (spawnInterval > 0f)
                yield return new WaitForSeconds(spawnInterval);
        }
    }

    private static Vector3 DirectionToVector(P2SpawnDirection dir)
    {
        switch (dir)
        {
            case P2SpawnDirection.Left: return Vector3.left;
            case P2SpawnDirection.Right: return Vector3.right;
            case P2SpawnDirection.Up: return Vector3.up;
            case P2SpawnDirection.Down: return Vector3.down;
            default: return Vector3.left;
        }
    }
}