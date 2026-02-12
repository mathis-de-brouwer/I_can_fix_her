using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Spawn Prefab Around Point")]
public sealed class P2SpawnPrefabAroundPointCardEffect : P2CardEffect
{
    [Header("Spawn")]
    [SerializeField] private int spawnCount = 3;

    [Header("Placement")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private float randomJitter = 0.25f;

    [Header("Rotation")]
    [SerializeField] private bool randomYaw;

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (card == null)
        {
            Debug.LogWarning($"{nameof(P2SpawnPrefabAroundPointCardEffect)}: card is null.");
            return;
        }

        if (card.prefab == null)
        {
            Debug.LogWarning($"{nameof(P2SpawnPrefabAroundPointCardEffect)}: '{card.cardName}' has no prefab assigned.");
            return;
        }

        Vector3 center;
        string centerSource;

        if (context != null && context.target != null)
        {
            center = context.target.position;
            centerSource = "target";
        }
        else if (context != null && context.spawnPoint != null)
        {
            center = context.spawnPoint.position;
            centerSource = "spawnPoint";
        }
        else
        {
            center = Vector3.zero;
            centerSource = "zero";
        }

        int count = Mathf.Max(1, spawnCount);

        Debug.Log($"{nameof(P2SpawnPrefabAroundPointCardEffect)}: Spawning {count}x '{card.prefab.name}' for '{card.cardName}' around {centerSource} at {center}.");

        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * Mathf.PI * 2f;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector3 pos = center + new Vector3(dir.x, dir.y, 0f) * radius;

            if (randomJitter > 0f)
            {
                pos += new Vector3(
                    Random.Range(-randomJitter, randomJitter),
                    Random.Range(-randomJitter, randomJitter),
                    0f);
            }

            Quaternion rot = randomYaw ? Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) : Quaternion.identity;
            Instantiate(card.prefab, pos, rot);
        }
    }
}