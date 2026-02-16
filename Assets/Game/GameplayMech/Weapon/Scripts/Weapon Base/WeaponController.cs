using UnityEngine;
//This is base script for all weapons (future weapons you gonna make idiot)
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData; 
    float currentCooldown;
    protected Playermovement pm;
    protected virtual void Start()
    {
        pm = FindAnyObjectByType<Playermovement>();
        currentCooldown = weaponData.CooldownDuration; // This shows that when player gets a weapon/new weapon it doesn't immediately starts shooting but the cooldown starts instead
    }

    
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f) //when cooldown hits 0, attack 
        {
            Attack();
        }
    }
//Basically when Attack gets called it loops the cooldown again why the (currentCooldwon = CooldownDuration)
    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration; 
    }
}
