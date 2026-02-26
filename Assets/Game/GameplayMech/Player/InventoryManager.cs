using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> WeaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<PassiveItems> PassiveItemSlots = new List<PassiveItems>(6);
    public int[] passiveItemLevels = new int[6];

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        if (weapon == null)
        {
            Debug.LogError($"InventoryManager.AddWeapon: WeaponController is missing (slotIndex={slotIndex}). " +
                           $"Ensure the weapon prefab has a WeaponController component.");
            return;
        }

        if (weapon.weaponData == null)
        {
            Debug.LogError($"InventoryManager.AddWeapon: weaponData is not assigned on '{weapon.gameObject.name}' (slotIndex={slotIndex}).");
            return;
        }

        WeaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
    }

    public void AddPassiveItem(int slotIndex, PassiveItems passiveItem)
    {
        PassiveItemSlots[slotIndex] = passiveItem;
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if (WeaponSlots.Count > slotIndex)
        {
            WeaponController weapon = WeaponSlots[slotIndex];
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, weapon.transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;
        }
    }

    public void LevelUpPassiveItem(int slotIndex)
    {
        if (PassiveItemSlots.Count > slotIndex)
        {
            PassiveItems passiveItem = PassiveItemSlots[slotIndex];
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemsData.NextLevelPrefab, passiveItem.transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItems>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItems>().passiveItemsData.Level;
        }
    }
}
