using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Double Monsters")]
public sealed class P2DoubleMonstersCardEffect : P2CardEffect
{
    [Header("Doubling")]
    [SerializeField] private int doubleCount = 3;

    [Tooltip("Small random offset so the clone doesn't overlap exactly.")]
    [SerializeField] private float spawnOffset = 0.6f;

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        EnemyStats[] all = Object.FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

        if (all.Length == 0)
        {
            Debug.Log($"{nameof(P2DoubleMonstersCardEffect)}: No enemies in scene to double.");
            return;
        }

        // Fisher-Yates shuffle into a temporary list so we pick randomly without repeats
        List<EnemyStats> pool = new List<EnemyStats>(all);
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            EnemyStats tmp = pool[i];
            pool[i] = pool[j];
            pool[j] = tmp;
        }

        int count = Mathf.Min(doubleCount, pool.Count);
        Debug.Log($"{nameof(P2DoubleMonstersCardEffect)}: Doubling {count} of {pool.Count} enemies.");

        for (int i = 0; i < count; i++)
        {
            EnemyStats source = pool[i];
            if (source == null)
                continue;

            Vector3 offset = new Vector3(
                Random.Range(-spawnOffset, spawnOffset),
                Random.Range(-spawnOffset, spawnOffset),
                0f);

            Instantiate(source.gameObject, source.transform.position + offset, source.transform.rotation);
        }
    }
}