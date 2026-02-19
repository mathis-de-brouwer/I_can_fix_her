using UnityEngine;
//This is base script for all weapons (future weapons you gonna make idiot)
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    float currentCooldown;
    protected Playermovement pm;
    protected AimController aimController;

    protected virtual void Start()
    {
        pm = FindAnyObjectByType<Playermovement>();
        aimController = FindAnyObjectByType<AimController>();
        currentCooldown = weaponData.CooldownDuration;
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f) //when cooldown hits 0, attack
        {
            Attack();
        }
    }

    // Helper: returns the best available aim direction — USE THIS in override Attack()
    protected Vector2 GetAimDirection()
    {
        if(aimController != null)
            return aimController.AimDirection;

        return pm != null ? pm.lastMoveDirection.normalized : Vector2.right;
    }

    //Basically when Attack gets called it loops the cooldown again
    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    }
}