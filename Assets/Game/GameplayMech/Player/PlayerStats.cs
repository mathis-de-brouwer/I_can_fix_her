using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterSciptableObject characterData;


    //current stats

    public float currentHealth;
    public float currentRecovery;
    public float currentMovementSpeed;
    public float currentMight;
    public float currentProjectileSpeed;
    public float currentMagnet;

    //Experience and level of the player
    [Header("Experience/level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    //Class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startlevel; 
        public int endlevel;
        public int experienceCapIncrease;
    }

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible; 

    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex; 
    public int passiveItemIndex;

    void Start()
    {
        //initializing the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;
    }
    public void Update()
    {
        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;

        }else if (isInvincible)
        {
            isInvincible = false;
        }

        Recovery();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach(LevelRange range in levelRanges)
            {
                if(level >= range.startlevel && level <= range.endlevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
        }
    }

    public void TakeDamage(float dmg)
    {
        //this is if the player isn't currently invincible, reduce health and start invincibility 
        if(!isInvincible)
        {
            currentHealth -= dmg;
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

        if(currentHealth <= 0)
        {
            Kill();
        }
        }
        
    }

    public void Kill()
    {
        Debug.Log("Killed the player");
    }

    //public GameObject firstPassiveItem, secondPassiveItem;
    void Awake()
    {
        inventory = GetComponent<InventoryManager>();

        currentHealth = characterData.Maxhealth;
        currentRecovery = characterData.Recovery;
        currentMight = characterData.Might;
        currentMovementSpeed = characterData.MoveSpeed;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;

        // SpawnPassiveItem(firstPassiveItem);
        // SpawnPassiveItem(secondPassiveItem);

    }

    public void SpawnWeapon ( GameObject weapon)
    {
        if(weaponIndex >= inventory.WeaponSlots.Count - 1)
        {
            Debug.LogWarning("No more weapon slots available!");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); //set the weapon to be a child of the player 
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); //add the weapon to the inventory manager's list of weapons

        weaponIndex++;
    }

        public void SpawnPassiveItem ( GameObject passiveItem)
    {
        if(passiveItemIndex >= inventory.PassiveItemSlots.Count - 1)
        {
            Debug.LogWarning("No more passive item slots available!");
            return;
        }
        
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //set the weapon to be a child of the player 
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItems>()); //add the passive item to the inventory manager's list of weapons

        passiveItemIndex++;
    }

    void Recovery()
    {
        if(currentHealth < characterData.Maxhealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
        
            // makes sure that the health of the player doesn't go over the maximum
            if( currentHealth > characterData.Maxhealth)
            {
                currentHealth = characterData.Maxhealth;
            }
        }
    }

    /// <summary>
    /// Grants invincibility for a given duration (used by the dash).
    /// Uses Mathf.Max so it never cuts short an existing I-frame window.
    /// </summary>
    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = Mathf.Max(invincibilityTimer, duration);
    }
}

