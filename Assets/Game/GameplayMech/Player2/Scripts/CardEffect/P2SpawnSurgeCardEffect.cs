using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Spawn Surge")]
public sealed class P2SpawnSurgeCardEffect : P2CardEffect
{
    [Header("Surge")]
    [SerializeField] private int waveSize = 50;
    [SerializeField] private float totalDuration = 20f;

    [Header("Placement")]
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private float randomJitter = 0.5f;

    [Header("Spawn table")]
    [Tooltip("Each entry is a prefab + weight. One entry = always that type. " +
             "Multiple entries = weighted random pick per spawn. Falls back to card.prefab if empty.")]
    [SerializeField] private List<P2SpawnEntry> spawnTable = new List<P2SpawnEntry>();

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (context == null || context.coroutineRunner == null)
        {
            Debug.LogWarning($"{nameof(P2SpawnSurgeCardEffect)}: no coroutine runner.");
            return;
        }

        if (!SpawnTable.IsValid(spawnTable) && (card == null || card.prefab == null))
        {
            Debug.LogWarning($"{nameof(P2SpawnSurgeCardEffect)}: no prefabs to spawn.");
            return;
        }

        context.coroutineRunner.StartCoroutine(SurgeRoutine(card, context));
    }

    private IEnumerator SurgeRoutine(P2Card card, P2CardEffectContext context)
    {
        float magnitudeMult = 1f;
        if (card != null && card.scaleMagnitudeWithTime && context.timeScaling != null)
            magnitudeMult = context.timeScaling.GetMagnitudeMultiplier(context.elapsedSeconds);

        int count = Mathf.Max(1, Mathf.CeilToInt(waveSize * magnitudeMult));
        float interval = totalDuration / count;

        Debug.Log($"{nameof(P2SpawnSurgeCardEffect)}: Surging {count} enemies over {totalDuration}s (every {interval:F2}s).");

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = SpawnTable.IsValid(spawnTable)
                ? SpawnTable.Pick(spawnTable)
                : card?.prefab;

            if (prefab != null)
            {
                Vector3 origin = context.target != null ? context.target.position
                               : context.spawnPoint != null ? context.spawnPoint.position
                               : Vector3.zero;

                float angle = Random.Range(0f, Mathf.PI * 2f);
                float dist = spawnRadius + Random.Range(-randomJitter, randomJitter);
                Vector3 pos = origin + new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);

                Instantiate(prefab, pos, Quaternion.identity);
            }

            yield return new WaitForSeconds(interval);
        }

        Debug.Log($"{nameof(P2SpawnSurgeCardEffect)}: Surge complete.");
    }
}