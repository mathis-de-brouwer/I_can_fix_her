using UnityEngine;

//This is base script for all weapons (future weapons you gonna make idiot)
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;

    [SerializeField] private float minCooldownDuration = 0.05f;

    float currentCooldown;
    protected Playermovement pm;
    protected AimController aimController;

    protected virtual void Start()
    {
        pm = FindAnyObjectByType<Playermovement>();
        aimController = FindAnyObjectByType<AimController>();

        currentCooldown = GetCooldownDuration();
    }

    protected virtual void Update()
    {
        if (weaponData == null)
            return;

        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) //when cooldown hits 0, attack
        {
            Attack();
        }
    }

    float GetCooldownDuration()
    {
        if (weaponData == null)
            return 9999f;

        return Mathf.Max(minCooldownDuration, weaponData.CooldownDuration);
    }

    // Helper: returns the best available aim direction — USE THIS in override Attack()
    protected Vector2 GetAimDirection()
    {
        if (aimController != null)
            return aimController.AimDirection;

        return pm != null ? pm.lastMoveDirection.normalized : Vector2.right;
    }

    //Basically when Attack gets called it loops the cooldown again
    protected virtual void Attack()
    {
        currentCooldown = GetCooldownDuration();
    }
}