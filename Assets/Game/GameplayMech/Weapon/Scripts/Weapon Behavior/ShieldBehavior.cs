using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    [Header("Shield Stats")]
    public float maxShieldHealth = 30f;
    public float shieldCooldown = 20f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer shieldSprite;
    [SerializeField] private Color fullHealthColor = new Color(0.3f, 0.8f, 1f, 0.5f);
    [SerializeField] private Color lowHealthColor = new Color(1f, 0.2f, 0.2f, 0.5f);

    private float _currentHealth;
    private float _cooldownTimer;
    private bool _shieldActive = true;

    void Start()
    {
        if (shieldSprite == null)
            shieldSprite = GetComponent<SpriteRenderer>();

        _currentHealth = maxShieldHealth;
        _shieldActive = true;
        UpdateVisual();
    }

    void Update()
    {
        if (!_shieldActive)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0f)
            {
                ReactivateShield();
            }
        }

        // Follow player
        Transform player = transform.parent;
        if (player != null)
            transform.position = player.position;
    }

    /// <summary>
    /// Called by PlayerStats when the player takes damage while shield is active.
    /// Returns the leftover damage that passes through to the player.
    /// </summary>
    public float AbsorbDamage(float incomingDamage)
    {
        if (!_shieldActive) return incomingDamage;

        _currentHealth -= incomingDamage;

        if (_currentHealth <= 0f)
        {
            float leftover = Mathf.Abs(_currentHealth); // damage that bleeds through
            BreakShield();
            return leftover;
        }

        Debug.Log($"Shield absorbed {incomingDamage} damage. Shield HP: {_currentHealth}/{maxShieldHealth}");
        UpdateVisual();
        return 0f; // all damage absorbed
    }

    void BreakShield()
    {
        _shieldActive = false;
        _cooldownTimer = shieldCooldown;

        if (shieldSprite != null)
            shieldSprite.enabled = false;

        Debug.Log($"Shield broken! Recharging in {shieldCooldown}s.");
    }

    void ReactivateShield()
    {
        _shieldActive = true;
        _currentHealth = maxShieldHealth;

        if (shieldSprite != null)
            shieldSprite.enabled = true;

        Debug.Log("Shield recharged!");
        UpdateVisual();
    }

    public bool IsShieldActive() => _shieldActive;

    void UpdateVisual()
    {
        if (shieldSprite == null) return;

        float healthPercent = _currentHealth / maxShieldHealth;
        shieldSprite.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);

        // Scale down slightly as health decreases for extra feedback
        float scale = Mathf.Lerp(0.8f, 1f, healthPercent);
        transform.localScale = Vector3.one * scale;
    }
}