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

    public List<LevelRange> levelRanges;

    void Start()
    {
        //initializing the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;
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

    void Awake()
    {
        currentHealth = characterData.Maxhealth;
        currentRecovery = characterData.Recovery;
        currentMight = characterData.Might;
        currentMovementSpeed = characterData.MoveSpeed;
        currentProjectileSpeed = characterData.ProjectileSpeed;
    }


}

