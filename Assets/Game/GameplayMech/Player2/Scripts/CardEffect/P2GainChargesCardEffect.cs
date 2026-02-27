using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Effects/Gain Charges")]
public sealed class P2GainChargesCardEffect : P2CardEffect
{
    [SerializeField] private bool useCardValue = true;
    [SerializeField] private float amount = 1f;

    public override void Resolve(P2Card card, P2CardEffectContext context)
    {
        if (context == null || context.charges == null)
            return;

        float gainAmount = useCardValue && card != null ? card.value : amount;
        gainAmount = Mathf.Max(0f, gainAmount);

        float magnitudeMult = 1f;
        if (card != null && card.scaleMagnitudeWithTime && context.timeScaling != null)
            magnitudeMult = context.timeScaling.GetMagnitudeMultiplier(context.elapsedSeconds);

        gainAmount *= magnitudeMult;

        Debug.Log($"GainCharges: +{gainAmount:F2} (before={context.charges.currentCharges:F2}, max={context.charges.maxCharges:F2})");

        context.charges.Gain(gainAmount);
    }
}