using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card")]
public class P2Card : ScriptableObject
{
    [Header("Info")]
    public string cardName;

    [TextArea(2, 6)]
    public string description;

    public Sprite icon;

    [Header("Cost & Timing")]
    public float cost = 1f;
    public float duration = 3f;

    [Header("Effect")]
    public P2CardEffect effect;

    [Header("Payload")]
    [Tooltip("Prefab spawned by effects such as Spawn Prefab, Spawn Wave, Place Trap.")]
    public GameObject prefab;

    [Tooltip("Generic scalar read by effects that need a magnitude: damage amount, buff multiplier, charge gain, etc. " +
             "What it means depends on the effect assigned above.")]
    public float value;

    [Header("Time scaling (optional)")]
    public bool scaleCostWithTime = true;
    public bool scaleMagnitudeWithTime = true;
}
