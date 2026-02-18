using UnityEngine;

/// <summary>
/// Attach to a trap prefab. Requires a Collider2D with Is Trigger = true.
/// When the player walks over it, deals damage once then destroys itself.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public sealed class P2DamageTrapBehaviour : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("Flat damage dealt to the player. Overridden at runtime by the card's value if > 0.")]
    public float damage = 1f;

    [Header("Visuals (optional)")]
    [Tooltip("Optional particle or effect spawned on trigger.")]
    [SerializeField] private GameObject hitVfxPrefab;

    private bool _triggered;

    private void Start()
    {
        // Catch misconfigured prefabs early
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"{nameof(P2DamageTrapBehaviour)} on '{gameObject.name}': Collider2D is not set to Is Trigger. The trap will not fire.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        _triggered = true;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null)
            stats.TakeDamage(damage);

        if (hitVfxPrefab != null)
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}