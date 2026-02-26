using UnityEngine;

public enum P1RewardOfferType
{
    NewPassive = 0,
    NewWeapon = 1,
    WeaponUpgrade = 2,
    PassiveUpgrade = 3,
}

public sealed class P1RewardOffer
{
    public P1RewardOfferType Type { get; }
    public GameObject Prefab { get; }
    public int SlotIndex { get; }

    public P1RewardOffer(P1RewardOfferType type, GameObject prefab, int slotIndex = -1)
    {
        Type = type;
        Prefab = prefab;
        SlotIndex = slotIndex;
    }
}