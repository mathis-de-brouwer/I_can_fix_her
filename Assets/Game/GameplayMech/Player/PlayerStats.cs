using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private ShieldBehavior _activeShield;

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

        RegisterStartingInventory();
    }

    void RegisterStartingInventory()
    {
        if (inventory == null)
            return;

        // Register starting weapons (children under the player).
        WeaponController[] weapons = GetComponentsInChildren<WeaponController>(true);

        int weaponSlot = 0;
        for (int i = 0; i < weapons.Length; i++)
        {
            WeaponController wc = weapons[i];
            if (wc == null || wc.weaponData == null)
                continue;

            // Don't double-register the same weapon controller.
            if (inventory.WeaponSlots.Contains(wc))
                continue;

            // Find next empty slot.
            while (weaponSlot < inventory.WeaponSlots.Count && inventory.WeaponSlots[weaponSlot] != null)
                weaponSlot++;

            if (weaponSlot >= inventory.WeaponSlots.Count)
                break;

            inventory.AddWeapon(weaponSlot, wc);
            weaponSlot++;
        }

        weaponIndex = FindFirstEmptyWeaponSlotIndex();

        // Register starting passives (optional; same idea).
        PassiveItems[] passives = GetComponentsInChildren<PassiveItems>(true);

        int passiveSlot = 0;
        for (int i = 0; i < passives.Length; i++)
        {
            PassiveItems pi = passives[i];
            if (pi == null || pi.passiveItemsData == null)
                continue;

            if (inventory.PassiveItemSlots.Contains(pi))
                continue;

            while (passiveSlot < inventory.PassiveItemSlots.Count && inventory.PassiveItemSlots[passiveSlot] != null)
                passiveSlot++;

            if (passiveSlot >= inventory.PassiveItemSlots.Count)
                break;

            inventory.AddPassiveItem(passiveSlot, pi);
            passiveSlot++;
        }

        passiveItemIndex = FindFirstEmptyPassiveSlotIndex();
    }

    int FindFirstEmptyWeaponSlotIndex()
    {
        for (int i = 0; i < inventory.WeaponSlots.Count; i++)
            if (inventory.WeaponSlots[i] == null)
                return i;

        return inventory.WeaponSlots.Count;
    }

    int FindFirstEmptyPassiveSlotIndex()
    {
        for (int i = 0; i < inventory.PassiveItemSlots.Count; i++)
            if (inventory.PassiveItemSlots[i] == null)
                return i;

        return inventory.PassiveItemSlots.Count;
    }

    void Update()
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
        while (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            experienceCap = CalculateNextExperienceCap(experienceCap);

            if (LevelUpRewardManager.Instance != null)
                LevelUpRewardManager.Instance.NotifyLevelUp(level);
        }
    }

    int CalculateNextExperienceCap(int currentCap)
    {
        int increase = 0;

        for (int i = 0; i < levelRanges.Count; i++)
        {
            LevelRange range = levelRanges[i];
            if (level >= range.startlevel && level <= range.endlevel)
            {
                increase = range.experienceCapIncrease;
                break;
            }
        }

        if (increase <= 0)
        {
            // Fallback so the cap always increases (in case ranges are misconfigured).
            increase = 1;
        }

        return currentCap + increase;
    }

    public void TakeDamage(float dmg)
    {
        if (_activeShield != null && _activeShield.IsShieldActive())
        {
            dmg = _activeShield.AbsorbDamage(dmg);
            if (dmg <= 0f) return; // shield absorbed everything
        }

        if (isDead) return;

        if (!isInvincible)
        {
            UiSfx.PlayPlayerHit();

            if (MatchStats.Instance != null)
                MatchStats.Instance.RegisterDamageToP1(dmg);

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

        UiSfx.PlayPlayerDeath();

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

    public void SetShield(ShieldBehavior shield)
    {
        _activeShield = shield;
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

