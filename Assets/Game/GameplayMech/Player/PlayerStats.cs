using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterSciptableObject characterData;


    //current stats

    float currentHealth;
    float currentRecovery;
    float currentMovementSpeed;
    float currentMight;
    float currentProjectileSpeed;

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

    void Awake()
    {
        currentHealth = characterData.Maxhealth;
        currentRecovery = characterData.Recovery;
        currentMight = characterData.Might;
        currentMovementSpeed = characterData.MoveSpeed;
        currentProjectileSpeed = characterData.ProjectileSpeed;
    }

    void Recovery()
    {
        if(currentHealth < characterData.Maxhealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
        }
    }

}

