using System.Collections.Generic;
using UnityEngine;

public class LevelUpRewardManager : MonoBehaviour
{
    public static LevelUpRewardManager Instance { get; private set; }

    [Header("Trigger")]
    [Tooltip("Reward fires every N levels (e.g. 3 = levels 3, 6, 9 ...).")]
    public int levelsPerReward = 3;

    [Header("Databases")]
    [SerializeField] private PassiveItemDatabase passiveItemDatabase;
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private P2CardDatabase cardDatabase;

    [Header("P1 Offer Mix")]
    [SerializeField, Range(0f, 1f)] float weightPassiveNew = 0.40f;
    [SerializeField, Range(0f, 1f)] float weightPassiveUpgrade = 0.20f;
    [SerializeField, Range(0f, 1f)] float weightWeaponNew = 0.15f;
    [SerializeField, Range(0f, 1f)] float weightWeaponUpgrade = 0.25f;

    [Header("Debug")]
    [SerializeField] bool debugOffers;

    [Header("Prefab")]
    [Tooltip("Drag the LevelUpRewardScreen prefab here — instantiated at runtime like the death screen.")]
    [SerializeField] private GameObject levelUpRewardScreenPrefab;

    bool _rewardActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NotifyLevelUp(int newLevel)
    {
        if (_rewardActive)
            return;

        if (newLevel % levelsPerReward != 0)
            return;

        ShowRewardScreen();
    }

    void ShowRewardScreen()
    {
        _rewardActive = true;

        SetP1GameplayInputEnabled(false);

        Time.timeScale = 0f;

        List<P1RewardOffer> p1Offers = BuildP1Offers(3);
        List<P2Card> p2Offers = PickRandom(cardDatabase.cards, 3);

        GameObject instance = Instantiate(levelUpRewardScreenPrefab);
        instance.GetComponent<LevelUpRewardScreenUI>().Setup(
            p1Offers,
            p2Offers,
            OnP1Chose,
            OnP2Chose
        );

        MusicService.Play(MusicId.LevelUp);
    }

    List<P1RewardOffer> BuildP1Offers(int count)
    {
        PlayerStats p1 = FindAnyObjectByType<PlayerStats>();
        InventoryManager inventory = p1 != null ? p1.GetComponent<InventoryManager>() : null;

        List<P1RewardOffer> results = new List<P1RewardOffer>(count);
        if (p1 == null || inventory == null)
            return results;

        List<int> upgradeableWeaponSlots = GetUpgradeableWeaponSlots(inventory);
        List<int> upgradeablePassiveSlots = GetUpgradeablePassiveSlots(inventory);

        bool canAddWeapon = p1.weaponIndex < inventory.WeaponSlots.Count - 1;
        bool canAddPassive = p1.passiveItemIndex < inventory.PassiveItemSlots.Count - 1;

        List<GameObject> passivePool = passiveItemDatabase != null ? passiveItemDatabase.passiveItemPrefabs : null;
        List<GameObject> weaponPool = weaponDatabase != null ? weaponDatabase.weaponPrefabs : null;

        HashSet<PassiveItemsScriptableObjects> ownedPassiveData = GetOwnedPassiveData(inventory);
        HashSet<WeaponScriptableObject> ownedWeaponData = GetOwnedWeaponData(inventory);

        List<GameObject> filteredPassivePool = FilterPassivePoolByData(passivePool, ownedPassiveData);
        List<GameObject> filteredWeaponPool = FilterWeaponPoolByData(weaponPool, ownedWeaponData);

        if (debugOffers)
        {
            Debug.Log(
                $"[LevelUpReward] pools: passive={passivePool?.Count ?? 0} (filtered={filteredPassivePool?.Count ?? 0}), " +
                $"weapon={weaponPool?.Count ?? 0} (filtered={filteredWeaponPool?.Count ?? 0}), " +
                $"upgradeWeaponSlots={upgradeableWeaponSlots.Count}, upgradePassiveSlots={upgradeablePassiveSlots.Count}, " +
                $"canAddPassive={canAddPassive}, canAddWeapon={canAddWeapon}");
        }

        HashSet<GameObject> usedPrefabs = new HashSet<GameObject>();
        HashSet<int> usedSlots = new HashSet<int>();

        for (int i = 0; i < count; i++)
        {
            P1RewardOffer offer = PickOneOffer(
                canAddPassive,
                canAddWeapon,
                upgradeablePassiveSlots,
                upgradeableWeaponSlots,
                usedPrefabs,
                usedSlots,
                filteredPassivePool,
                filteredWeaponPool,
                inventory);

            if (offer != null)
                results.Add(offer);
        }

        if (results.Count == 0 && debugOffers)
            Debug.LogWarning("[LevelUpReward] Generated 0 P1 offers. Check databases, filtering, and slot availability.");

        // Keep your hard fallback as-is (not repeated here for brevity)
        if (results.Count == 0)
        {
            if (upgradeableWeaponSlots.Count > 0)
                results.Add(BuildWeaponUpgradeOffer(inventory, upgradeableWeaponSlots[0]));
            else if (upgradeablePassiveSlots.Count > 0)
                results.Add(BuildPassiveUpgradeOffer(inventory, upgradeablePassiveSlots[0]));
            else if (filteredPassivePool != null && filteredPassivePool.Count > 0)
                results.Add(new P1RewardOffer(P1RewardOfferType.NewPassive, filteredPassivePool[0]));
            else if (filteredWeaponPool != null && filteredWeaponPool.Count > 0)
                results.Add(new P1RewardOffer(P1RewardOfferType.NewWeapon, filteredWeaponPool[0]));
        }

        return results;
    }

    P1RewardOffer PickOneOffer(
        bool canAddPassive,
        bool canAddWeapon,
        List<int> upgradeablePassiveSlots,
        List<int> upgradeableWeaponSlots,
        HashSet<GameObject> usedPrefabs,
        HashSet<int> usedSlots,
        List<GameObject> passivePool,
        List<GameObject> weaponPool,
        InventoryManager inventory)
    {
        List<(P1RewardOfferType type, float weight)> candidates = new List<(P1RewardOfferType, float)>(4);

        if (canAddPassive && passivePool != null && passivePool.Count > 0)
            candidates.Add((P1RewardOfferType.NewPassive, weightPassiveNew));

        if (canAddWeapon && weaponPool != null && weaponPool.Count > 0)
            candidates.Add((P1RewardOfferType.NewWeapon, weightWeaponNew));

        if (upgradeablePassiveSlots != null && upgradeablePassiveSlots.Count > 0)
            candidates.Add((P1RewardOfferType.PassiveUpgrade, weightPassiveUpgrade));

        if (upgradeableWeaponSlots != null && upgradeableWeaponSlots.Count > 0)
            candidates.Add((P1RewardOfferType.WeaponUpgrade, weightWeaponUpgrade));

        if (candidates.Count == 0)
            return null;

        // Attempt a few times to avoid duplicates in the same screen.
        for (int attempt = 0; attempt < 10; attempt++)
        {
            P1RewardOfferType pickedType = WeightedPick(candidates);

            switch (pickedType)
            {
                case P1RewardOfferType.NewPassive:
                {
                    P1RewardOffer offer = PickNewPrefabOffer(P1RewardOfferType.NewPassive, passivePool, usedPrefabs);
                    if (offer != null) return offer;
                    break;
                }

                case P1RewardOfferType.NewWeapon:
                {
                    P1RewardOffer offer = PickNewPrefabOffer(P1RewardOfferType.NewWeapon, weaponPool, usedPrefabs);
                    if (offer != null) return offer;
                    break;
                }

                case P1RewardOfferType.PassiveUpgrade:
                {
                    int slotIndex = PickUnusedSlot(upgradeablePassiveSlots, usedSlots);
                    if (slotIndex >= 0) return BuildPassiveUpgradeOffer(inventory, slotIndex);
                    break;
                }

                case P1RewardOfferType.WeaponUpgrade:
                {
                    int slotIndex = PickUnusedSlot(upgradeableWeaponSlots, usedSlots);
                    if (slotIndex >= 0) return BuildWeaponUpgradeOffer(inventory, slotIndex);
                    break;
                }
            }
        }

        return null;
    }

    static int PickUnusedSlot(List<int> slots, HashSet<int> usedSlots)
    {
        if (slots == null || slots.Count == 0)
            return -1;

        for (int tries = 0; tries < 12; tries++)
        {
            int candidate = slots[Random.Range(0, slots.Count)];
            if (usedSlots.Contains(candidate)) continue;

            usedSlots.Add(candidate);
            return candidate;
        }

        return -1;
    }

    P1RewardOffer PickNewPrefabOffer(P1RewardOfferType type, List<GameObject> pool, HashSet<GameObject> usedPrefabs)
    {
        if (pool == null || pool.Count == 0)
            return null;

        // Build list of unused prefabs for this screen, then pick from that.
        List<GameObject> available = new List<GameObject>();
        for (int i = 0; i < pool.Count; i++)
        {
            GameObject prefab = pool[i];
            if (prefab == null) continue;
            if (usedPrefabs.Contains(prefab)) continue;
            available.Add(prefab);
        }

        if (available.Count == 0)
            return null;

        GameObject picked = available[Random.Range(0, available.Count)];
        usedPrefabs.Add(picked);
        return new P1RewardOffer(type, picked);
    }

    static P1RewardOffer BuildWeaponUpgradeOffer(InventoryManager inventory, int slotIndex)
    {
        WeaponController wc = inventory != null && slotIndex >= 0 && slotIndex < inventory.WeaponSlots.Count
            ? inventory.WeaponSlots[slotIndex]
            : null;

        if (wc == null)
            return null;

        return new P1RewardOffer(P1RewardOfferType.WeaponUpgrade, wc.gameObject, slotIndex);
    }

    static P1RewardOffer BuildPassiveUpgradeOffer(InventoryManager inventory, int slotIndex)
    {
        PassiveItems pi = inventory != null && slotIndex >= 0 && slotIndex < inventory.PassiveItemSlots.Count
            ? inventory.PassiveItemSlots[slotIndex]
            : null;

        if (pi == null)
            return null;

        return new P1RewardOffer(P1RewardOfferType.PassiveUpgrade, pi.gameObject, slotIndex);
    }

    static List<int> GetUpgradeableWeaponSlots(InventoryManager inventory)
    {
        List<int> slots = new List<int>();

        if (inventory == null || inventory.WeaponSlots == null)
            return slots;

        for (int i = 0; i < inventory.WeaponSlots.Count; i++)
        {
            WeaponController wc = inventory.WeaponSlots[i];
            if (wc == null) continue;

            WeaponScriptableObject data = wc.weaponData;
            if (data == null) continue;

            if (data.NextLevelPrefab != null)
                slots.Add(i);
        }

        return slots;
    }

    static List<int> GetUpgradeablePassiveSlots(InventoryManager inventory)
    {
        List<int> slots = new List<int>();

        if (inventory == null || inventory.PassiveItemSlots == null)
            return slots;

        for (int i = 0; i < inventory.PassiveItemSlots.Count; i++)
        {
            PassiveItems pi = inventory.PassiveItemSlots[i];
            if (pi == null) continue;

            PassiveItemsScriptableObjects data = pi.passiveItemsData;
            if (data == null) continue;

            if (data.NextLevelPrefab != null)
                slots.Add(i);
        }

        return slots;
    }

    static HashSet<GameObject> GetOwnedWeaponIdentityPrefabs(InventoryManager inventory)
    {
        HashSet<GameObject> owned = new HashSet<GameObject>();
        if (inventory == null || inventory.WeaponSlots == null)
            return owned;

        for (int i = 0; i < inventory.WeaponSlots.Count; i++)
        {
            WeaponController wc = inventory.WeaponSlots[i];
            if (wc == null || wc.weaponData == null) continue;

            // Identity is weaponData.Prefab (base line). If not set, fall back to current instance prefab reference.
            if (wc.weaponData.Prefab != null)
                owned.Add(wc.weaponData.Prefab);
        }

        return owned;
    }

    static HashSet<GameObject> GetOwnedPassiveIdentityPrefabs(InventoryManager inventory)
    {
        HashSet<GameObject> owned = new HashSet<GameObject>();
        if (inventory == null || inventory.PassiveItemSlots == null)
            return owned;

        for (int i = 0; i < inventory.PassiveItemSlots.Count; i++)
        {
            PassiveItems pi = inventory.PassiveItemSlots[i];
            if (pi == null || pi.passiveItemsData == null) continue;

            // Identity is the prefab referenced by the scriptable object next-level chain root.
            // Since scriptable object doesn't store a base prefab, we use the current prefab instance name identity indirectly.
            // Easiest stable identity: the ScriptableObject reference itself.
            // For filtering from prefab pool, we will compare prefab's passiveItemsData vs owned data elsewhere if needed.
        }

        return owned;
    }

    static List<GameObject> FilterNewPool(List<GameObject> pool, HashSet<GameObject> ownedIdentityPrefabs)
    {
        if (pool == null)
            return null;

        List<GameObject> filtered = new List<GameObject>();
        for (int i = 0; i < pool.Count; i++)
        {
            GameObject prefab = pool[i];
            if (prefab == null) continue;

            // If ownedIdentityPrefabs is empty we just accept all.
            if (ownedIdentityPrefabs != null && ownedIdentityPrefabs.Count > 0 && ownedIdentityPrefabs.Contains(prefab))
                continue;

            filtered.Add(prefab);
        }

        return filtered;
    }

    static HashSet<PassiveItemsScriptableObjects> GetOwnedPassiveData(InventoryManager inventory)
    {
        HashSet<PassiveItemsScriptableObjects> owned = new HashSet<PassiveItemsScriptableObjects>();

        if (inventory == null || inventory.PassiveItemSlots == null)
            return owned;

        for (int i = 0; i < inventory.PassiveItemSlots.Count; i++)
        {
            PassiveItems item = inventory.PassiveItemSlots[i];
            if (item == null || item.passiveItemsData == null) continue;

            owned.Add(item.passiveItemsData);
        }

        return owned;
    }

    static HashSet<WeaponScriptableObject> GetOwnedWeaponData(InventoryManager inventory)
    {
        HashSet<WeaponScriptableObject> owned = new HashSet<WeaponScriptableObject>();

        if (inventory == null || inventory.WeaponSlots == null)
            return owned;

        for (int i = 0; i < inventory.WeaponSlots.Count; i++)
        {
            WeaponController weapon = inventory.WeaponSlots[i];
            if (weapon == null || weapon.weaponData == null) continue;

            owned.Add(weapon.weaponData);
        }

        return owned;
    }

    static List<GameObject> FilterPassivePoolByData(List<GameObject> pool, HashSet<PassiveItemsScriptableObjects> ownedData)
    {
        if (pool == null)
            return null;

        List<GameObject> filtered = new List<GameObject>(pool.Count);

        for (int i = 0; i < pool.Count; i++)
        {
            GameObject prefab = pool[i];
            if (prefab == null) continue;

            PassiveItems passive = prefab.GetComponent<PassiveItems>();
            PassiveItemsScriptableObjects data = passive != null ? passive.passiveItemsData : null;

            // If prefab has no data, keep it (or skip if you prefer strictness).
            if (data == null)
            {
                filtered.Add(prefab);
                continue;
            }

            // New passive offers must start at level 1.
            if (data.Level > 1)
                continue;

            if (ownedData != null && ownedData.Contains(data))
                continue;

            filtered.Add(prefab);
        }

        return filtered;
    }

    static List<GameObject> FilterWeaponPoolByData(List<GameObject> pool, HashSet<WeaponScriptableObject> ownedData)
    {
        if (pool == null)
            return null;

        List<GameObject> filtered = new List<GameObject>(pool.Count);

        for (int i = 0; i < pool.Count; i++)
        {
            GameObject prefab = pool[i];
            if (prefab == null) continue;

            WeaponController weapon = prefab.GetComponent<WeaponController>();
            WeaponScriptableObject data = weapon != null ? weapon.weaponData : null;

            if (data == null)
            {
                filtered.Add(prefab);
                continue;
            }

            // New weapon offers must start at level 1.
            if (data.Level > 1)
                continue;

            if (ownedData != null && ownedData.Contains(data))
                continue;

            filtered.Add(prefab);
        }

        return filtered;
    }

    static P1RewardOfferType WeightedPick(List<(P1RewardOfferType type, float weight)> items)
    {
        float total = 0f;
        for (int i = 0; i < items.Count; i++)
            total += Mathf.Max(0f, items[i].weight);

        if (total <= 0f)
            return items[Random.Range(0, items.Count)].type;

        float roll = Random.Range(0f, total);
        float running = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            running += Mathf.Max(0f, items[i].weight);
            if (roll <= running)
                return items[i].type;
        }

        return items[items.Count - 1].type;
    }

    void OnP1Chose(P1RewardOffer offer)
    {
        PlayerStats p1 = FindAnyObjectByType<PlayerStats>();
        InventoryManager inventory = p1 != null ? p1.GetComponent<InventoryManager>() : null;

        if (p1 == null || inventory == null || offer == null)
            return;

        switch (offer.Type)
        {
            case P1RewardOfferType.NewPassive:
                if (offer.Prefab != null)
                    p1.SpawnPassiveItem(offer.Prefab);
                break;

            case P1RewardOfferType.NewWeapon:
                if (offer.Prefab != null)
                    p1.SpawnWeapon(offer.Prefab);
                break;

            case P1RewardOfferType.WeaponUpgrade:
                if (offer.SlotIndex >= 0)
                    inventory.LevelUpWeapon(offer.SlotIndex);
                break;

            case P1RewardOfferType.PassiveUpgrade:
                if (offer.SlotIndex >= 0)
                    inventory.LevelUpPassiveItem(offer.SlotIndex);
                break;
        }
    }

    void OnP2Chose(P2Card card)
    {
        P2DeckManager deck = FindAnyObjectByType<P2DeckManager>();
        if (deck != null)
        {
            deck.AddCardToDeck(card);
            deck.AddCardToDeck(card); // ×2 copies
        }
    }

    void SetP1GameplayInputEnabled(bool enabled)
    {
        Playermovement movement = FindAnyObjectByType<Playermovement>();
        if (movement != null)
        {
            movement.InputLocked = !enabled;

            if (enabled) movement.playerControlls.Enable();
            else movement.playerControlls.Disable();

            if (enabled) movement.dashAction.Enable();
            else movement.dashAction.Disable();
        }

        AimController aim = FindAnyObjectByType<AimController>();
        if (aim != null)
        {
            if (enabled) aim.aimAction.Enable();
            else aim.aimAction.Disable();

            if (enabled) aim.aimUp.Enable();
            else aim.aimUp.Disable();

            if (enabled) aim.aimDown.Enable();
            else aim.aimDown.Disable();

            if (enabled) aim.aimLeft.Enable();
            else aim.aimLeft.Disable();

            if (enabled) aim.aimRight.Enable();
            else aim.aimRight.Disable();
        }
    }

    public void OnRewardScreenClosed()
    {
        Time.timeScale = 1f;
        SetP1GameplayInputEnabled(true);
        _rewardActive = false;

        MusicService.Play(MusicId.Gameplay);
    }

    static List<T> PickRandom<T>(List<T> source, int count)
    {
        List<T> pool = new List<T>(source);
        List<T> result = new List<T>();
        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return result;
    }
}