using UnityEngine;

public class ShieldController : WeaponController
{
    private ShieldBehavior _shield;

    protected override void Start()
    {
        base.Start();

        // Spawn the shield immediately as a child of the player
        if (weaponData != null && weaponData.Prefab != null)
        {
            GameObject shieldObj = Instantiate(weaponData.Prefab, transform.parent);
            shieldObj.transform.localPosition = Vector3.zero;
            _shield = shieldObj.GetComponent<ShieldBehavior>();

            // Register the shield with PlayerStats so TakeDamage can use it
            PlayerStats playerStats = GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.SetShield(_shield);
            }
        }
    }

    protected override void Attack()
    {
        // Shield doesn't attack — it's passive
        // Just reset cooldown so Update doesn't spam
        base.Attack();
    }
}