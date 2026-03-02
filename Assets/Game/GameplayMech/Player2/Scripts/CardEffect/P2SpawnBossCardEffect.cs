using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Spawn Boss")]
public sealed class P2SpawnBossCardEffect : P2CardEffect
{
    [Header("Placement")]
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private float randomJitter = 0.5f;

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (card == null)
            return;

        if (card.prefab == null)
        {
            Debug.LogWarning($"{nameof(P2SpawnBossCardEffect)}: '{card.cardName}' has no prefab assigned.");
            return;
        }

        Vector3 origin = context != null && context.target != null
            ? context.target.position
            : context != null && context.spawnPoint != null
                ? context.spawnPoint.position
                : Vector3.zero;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float dist = spawnRadius + Random.Range(-randomJitter, randomJitter);

        Vector3 pos = origin + new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);

        GameObject boss = Instantiate(card.prefab, pos, Quaternion.identity);

        P2DeckManager deckManager = Object.FindAnyObjectByType<P2DeckManager>();
        if (deckManager != null)
            deckManager.NotifyBossSpawned();

        P2BossDeathHook hook = boss.AddComponent<P2BossDeathHook>();
        hook.Initialize(deckManager);
    }
}