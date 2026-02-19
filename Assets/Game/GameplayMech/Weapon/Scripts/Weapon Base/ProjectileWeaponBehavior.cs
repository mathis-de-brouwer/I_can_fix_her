using Unity.VisualScripting;
using UnityEngine;

public class ProjectileWeaponBehavior : MonoBehaviour
{
     //Base script for projectile based weapons

     public WeaponScriptableObject weaponData;
     protected Vector3 direction; 
     public float destroyAfterSeconds;

     //Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

   void Awake()
    {
        currentDamage = weaponData.damage;
        currentSpeed = weaponData.speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.pierce;
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindAnyObjectByType<PlayerStats>().currentMight; // this way if there's a damage multiplier it doesn't mess up the base damage of the weapon
    } 
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if(dirx < 0 && diry == 0)
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }        else if (dirx == 0 && diry < 0) //down
        {
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry > 0) //up
        {
            scale.x = scale.x * -1;
        }
        else if (dirx > 0 && diry > 0) //right up
        {
            rotation.z = 0f;
        }
        else if (dirx > 0 && diry < 0) //right down
        {
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry > 0) //left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry < 0) //left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation); // can't simply set the vector because can't convert
    }

    protected virtual void OnTriggerEnter2D(Collider2D col) {
        // Refference the script from the collided collider and deal damage using TakeDamage();
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage()); // we use current damage instead of weapondata.damage so that if there's damage multiplier it doesn't mess up everything 
            ReducePierce();
        }
        // else if (col.CompareTag("Prop"))
        // {
        //     if(col.gameObject.TryGetComponent(out BreakableProps breakable))
        //     {
        //         breakable.TakeDamage(GetCurrentDamage());
        //         ReducePierce();
        //     }
        // }
    }

    void ReducePierce() //Destroy once the pierce reaches 0
    {
        currentPierce--;
        if(currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
