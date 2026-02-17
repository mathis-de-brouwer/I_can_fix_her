using UnityEngine;

public abstract class P2CardEffect : ScriptableObject
{
    [Header("Card visuals")]
    [SerializeField] private Sprite cardArtOverride;

    public Sprite CardArtOverride => cardArtOverride;

    public abstract void Resolve(P2Card card, P2CardEffectContext context);
}
