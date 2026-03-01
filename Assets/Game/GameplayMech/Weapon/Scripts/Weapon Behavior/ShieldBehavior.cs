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
        EnsureSpriteReference();

        _currentHealth = maxShieldHealth;
        _shieldActive = true;

        if (shieldSprite != null)
            shieldSprite.enabled = true;

        UpdateVisual();
    }

    void Update()
    {
        if (!_shieldActive)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0f)
                ReactivateShield();
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
        if (!_shieldActive)
            return incomingDamage;

        _currentHealth -= incomingDamage;

        if (_currentHealth <= 0f)
        {
            float leftover = Mathf.Abs(_currentHealth);
            BreakShield();
            return leftover;
        }

        UpdateVisual();
        return 0f;
    }

    void BreakShield()
    {
        _shieldActive = false;
        _cooldownTimer = shieldCooldown;

        EnsureSpriteReference();
        if (shieldSprite != null)
            shieldSprite.enabled = false;
    }

    void ReactivateShield()
    {
        _shieldActive = true;
        _currentHealth = maxShieldHealth;

        EnsureSpriteReference();
        if (shieldSprite != null)
        {
            shieldSprite.enabled = true;
            shieldSprite.color = fullHealthColor;
        }

        transform.localScale = Vector3.one;
        UpdateVisual();
    }

    public bool IsShieldActive() => _shieldActive;

    void UpdateVisual()
    {
        EnsureSpriteReference();
        if (shieldSprite == null)
            return;

        float healthPercent = _currentHealth / maxShieldHealth;
        shieldSprite.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);

        float scale = Mathf.Lerp(4f, 4.1f, healthPercent);
        transform.localScale = Vector3.one * scale;
    }

    void EnsureSpriteReference()
    {
        if (shieldSprite != null)
            return;

        shieldSprite = GetComponent<SpriteRenderer>();
        if (shieldSprite != null)
            return;

        shieldSprite = GetComponentInChildren<SpriteRenderer>(true);
    }
}