using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Place Traps Around Player")]
public sealed class P2PlaceTrapsAroundPlayerCardEffect : P2CardEffect
{
    [Header("Trap count")]
    [SerializeField] private int trapCount = 4;

    [Header("Placement")]
    [SerializeField] private float minRadius = 1f;
    [SerializeField] private float maxRadius = 3f;

    [Header("Trap prefab override (optional)")]
    [Tooltip("If set, uses this prefab instead of card.prefab.")]
    [SerializeField] private GameObject trapPrefabOverride;

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (card == null)
        {
            Debug.LogWarning($"{nameof(P2PlaceTrapsAroundPlayerCardEffect)}: card is null.");
            return;
        }

        GameObject prefab = trapPrefabOverride != null ? trapPrefabOverride : card.prefab;

        if (prefab == null)
        {
            Debug.LogWarning($"{nameof(P2PlaceTrapsAroundPlayerCardEffect)}: '{card.cardName}' has no trap prefab.");
            return;
        }

        // Place around the player (context.target), fall back to spawnPoint
        Vector3 center = context?.target != null ? context.target.position
                       : context?.spawnPoint != null ? context.spawnPoint.position
                       : Vector3.zero;

        int count = Mathf.Max(1, trapCount);
        Debug.Log($"{nameof(P2PlaceTrapsAroundPlayerCardEffect)}: Placing {count} traps around {center}.");

        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist  = Random.Range(minRadius, maxRadius);
            Vector3 pos = center + new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);

            GameObject trap = Instantiate(prefab, pos, Quaternion.identity);

            // Push card.value into the trap's damage if it was set
            if (card.value > 0f)
            {
                P2DamageTrapBehaviour behaviour = trap.GetComponent<P2DamageTrapBehaviour>();
                if (behaviour != null)
                    behaviour.damage = card.value;
            }
        }
    }
}