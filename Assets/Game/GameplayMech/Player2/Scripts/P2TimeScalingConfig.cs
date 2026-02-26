using UnityEngine;

[CreateAssetMenu(menuName = "P2/Time Scaling Config")]
public sealed class P2TimeScalingConfig : ScriptableObject
{
    [Header("Curves output multipliers. X = elapsed seconds, Y = multiplier.")]
    [SerializeField] private AnimationCurve costMultiplier = AnimationCurve.Linear(0f, 1f, 180f, 1f);
    [SerializeField] private AnimationCurve magnitudeMultiplier = AnimationCurve.Linear(0f, 1f, 180f, 1f);

    [Header("Safety clamps")]
    [SerializeField] private float minCostMultiplier = 0.1f;
    [SerializeField] private float maxCostMultiplier = 10f;
    [SerializeField] private float minMagnitudeMultiplier = 0.1f;
    [SerializeField] private float maxMagnitudeMultiplier = 10f;

    public float GetCostMultiplier(float elapsedSeconds)
    {
        float v = costMultiplier != null ? costMultiplier.Evaluate(elapsedSeconds) : 1f;
        return Mathf.Clamp(v, minCostMultiplier, maxCostMultiplier);
    }

    public float GetMagnitudeMultiplier(float elapsedSeconds)
    {
        float v = magnitudeMultiplier != null ? magnitudeMultiplier.Evaluate(elapsedSeconds) : 1f;
        return Mathf.Clamp(v, minMagnitudeMultiplier, maxMagnitudeMultiplier);
    }
}