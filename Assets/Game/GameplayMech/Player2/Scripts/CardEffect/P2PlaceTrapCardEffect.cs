using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Place Trap")]
public sealed class P2PlaceTrapCardEffect : P2CardEffect
{
    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (card == null || card.prefab == null)
            return;

        Vector3 pos = context != null && context.spawnPoint != null ? context.spawnPoint.position : Vector3.zero;
        Instantiate(card.prefab, pos, Quaternion.identity);
    }
}