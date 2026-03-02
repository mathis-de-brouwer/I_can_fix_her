using UnityEngine;

public class ShieldController : WeaponController
{
    private ShieldBehavior _shield;

    [Header("Level scaling")]
    [SerializeField] private float shieldHealthPerLevel = 10f;
    [SerializeField] private float cooldownReductionPerLevel = 2f;
    [SerializeField] private float minShieldCooldown = 2f;

    protected override void Start()
    {
        base.Start();

        // Spawn the shield immediately as a child of the player
        if (weaponData != null && weaponData.Prefab != null)
        {
            GameObject shieldObj = Instantiate(weaponData.Prefab, transform.parent);
            shieldObj.transform.localPosition = Vector3.zero;
            _shield = shieldObj.GetComponent<ShieldBehavior>();

            ApplyLevelScaling();

            // Register the shield with PlayerStats so TakeDamage can use it
            PlayerStats playerStats = GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.SetShield(_shield);
            }
        }
    }

    private void ApplyLevelScaling()
    {
        if (_shield == null || weaponData == null)
            return;

        int level = Mathf.Max(1, weaponData.Level);

        _shield.maxShieldHealth += shieldHealthPerLevel * (level - 1);
        _shield.shieldCooldown = Mathf.Max(minShieldCooldown, _shield.shieldCooldown - (cooldownReductionPerLevel * (level - 1)));
    }

    protected override void Attack()
    {
        // Shield doesn't attack — it's passive
        // Just reset cooldown so Update doesn't spam
        base.Attack();
    }
}