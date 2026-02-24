using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public CharacterSciptableObject characterData;
    public Slider healthSlider;

    [Header("Hurt / Death")]
    public Animator animator;
    public GameObject gameResultPrefab; // Drag the GameResult prefab here

    public float currentHealth;
    public float currentRecovery;
    public float currentMovementSpeed;
    public float currentMight;
    public float currentProjectileSpeed;
    public float currentMagnet;

    [Header("Experience/level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startlevel;
        public int endlevel;
        public int experienceCapIncrease;
    }

    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    bool isDead = false;

    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    void Awake()
    {
        inventory = GetComponent<InventoryManager>();

        currentHealth = characterData.Maxhealth;
        currentRecovery = characterData.Recovery;
        currentMight = characterData.Might;
        currentMovementSpeed = characterData.MoveSpeed;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;

        healthSlider.maxValue = characterData.Maxhealth;
        healthSlider.value = currentHealth;
    }

    public void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
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
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startlevel && level <= range.endlevel)
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
        if (isDead) return;

        if (!isInvincible)
        {
            currentHealth -= dmg;
            healthSlider.value = currentHealth;
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                Kill();
            }
            else
            {
                if (animator != null)
                    animator.SetTrigger("Hurt");
            }
        }
    }

    public void Kill()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        Playermovement movement = GetComponent<Playermovement>();
        if (movement != null)
            movement.enabled = false;

        StartCoroutine(ShowDeathScreen(1f));
    }

    IEnumerator ShowDeathScreen(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (gameResultPrefab != null)
        {
            GameObject instance = Instantiate(gameResultPrefab);
            instance.GetComponent<GameResultScreenUI>().Setup(GameResultScreenUI.Winner.P2);
        }
        else
        {
            Debug.LogWarning("PlayerStats: gameResultPrefab is not assigned!");
        }
    }

    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = Mathf.Max(invincibilityTimer, duration);
    }

    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventory.WeaponSlots.Count - 1)
        {
            Debug.LogWarning("No more weapon slots available!");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.PassiveItemSlots.Count - 1)
        {
            Debug.LogWarning("No more passive item slots available!");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItems>());
        passiveItemIndex++;
    }

    void Recovery()
    {
        if (isDead) return;

        if (currentHealth < characterData.Maxhealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
            healthSlider.value = currentHealth;

            if (currentHealth > characterData.Maxhealth)
                currentHealth = characterData.Maxhealth;
        }
    }
}

