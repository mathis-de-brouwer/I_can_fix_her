using UnityEngine;
//This is base script for all weapons (future weapons you gonna make idiot)
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float CooldownDuration;
    float currentCooldown;
    public int pierce;  //This is the max amount of times a weapon can touch an enemy before disappearing (like can only touch 1 or 2 enemy's before disappearing )
    protected Playermovement pm;
    protected virtual void Start()
    {
        pm = FindAnyObjectByType<Playermovement>();
        currentCooldown = CooldownDuration; // This shows that when player gets a weapon/new weapon it doesn't immediately starts shooting but the cooldown starts instead
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
        currentCooldown = CooldownDuration; 
    }
}
