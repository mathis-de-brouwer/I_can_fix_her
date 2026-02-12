using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Spawn Prefab")]
public sealed class P2SpawnPrefabCardEffect : P2CardEffect
{
    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (card == null || card.prefab == null)
            return;

        Vector3 spawnPos = context != null && context.spawnPoint != null ? context.spawnPoint.position : Vector3.zero;
        Instantiate(card.prefab, spawnPos, Quaternion.identity);
    }
}