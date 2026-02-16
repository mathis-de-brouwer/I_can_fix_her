using UnityEngine;

public abstract class P2CardEffect : ScriptableObject
{
    public abstract void Resolve(P2Card card, P2CardEffectContext context);
}
