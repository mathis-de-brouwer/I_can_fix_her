using UnityEngine;

public class P2CardEffectResolver : MonoBehaviour
{
    [Header("Default context")]
    [SerializeField] private Transform defaultSpawnPoint;

    [Header("Optional references")]
    [SerializeField] private P2Charges charges;

    [Header("Target (P1 survivor)")]
    [Tooltip("If null, will try to find Playermovement in scene at runtime.")]
    [SerializeField] private Playermovement targetPlayer;

    private readonly P2CardEffectContext _context = new P2CardEffectContext();

    private void Awake()
    {
        if (targetPlayer == null)
            targetPlayer = FindAnyObjectByType<Playermovement>();
    }

    public void Resolve(P2Card card)
    {
        if (card == null)
            return;

        if (card.effect == null)
        {
            Debug.LogWarning($"Card '{card.cardName}' has no effect assigned.");
            return;
        }

        _context.spawnPoint = defaultSpawnPoint;
        _context.charges = charges;
        _context.coroutineRunner = this;
        _context.target = targetPlayer != null ? targetPlayer.transform : null;

        card.effect.Resolve(card, _context);
    }
}