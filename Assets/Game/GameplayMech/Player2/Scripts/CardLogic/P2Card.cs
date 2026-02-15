using UnityEngine;

public enum CardType
{
    Spawn,
    Trap,
    Debuff,
    Pressure
}

[CreateAssetMenu(menuName = "P2/Card")]
public class P2Card : ScriptableObject
{
    [Header("Info")]
    public string cardName;
    public Sprite icon;

    [Header("Cost & Timing")]
    public float cost = 1f;
    public float cooldown = 2f;
    public float duration = 3f;

    [Header("Type")]
    public CardType type;

    [Header("Effect")]
    public P2CardEffect effect;

    [Header("Payload")]
    public GameObject prefab;   // used by some effects (ex. Spawn Prefab)
    public float value;         // used by some effects (ex. Debuff / Pressure)
}
