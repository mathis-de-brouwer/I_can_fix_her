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

    [Header("Payload")]
    public GameObject prefab;   // used for Spawn / Trap
    public float value;         // used for Debuff / Pressure
}
