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
        WeaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
    }

    public void AddPassiveItem(int slotIndex, PassiveItems passiveItem)
    {
        PassiveItemSlots[slotIndex] = passiveItem;
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if(WeaponSlots.Count > slotIndex)
        {
            WeaponController weapon = WeaponSlots[slotIndex];
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, weapon.transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); //set the weapon to be a child of the player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>()); //add the upgraded weapon to the inventory manager's list of weapons
            Destroy(weapon.gameObject); //destroy the old weapon
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; //update the weapon level in the inventory manager
        }
        
    }


    public void LevelUpPassiveItem(int slotIndex)
    {
        if(PassiveItemSlots.Count > slotIndex)
        {
            PassiveItems passiveItem = PassiveItemSlots[slotIndex];
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemsData.NextLevelPrefab, passiveItem.transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform); //set the passive item to be a child of the player
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItems>()); //add the upgraded passive item to the inventory manager's list of passive items
            Destroy(passiveItem.gameObject); //destroy the old passive item
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItems>().passiveItemsData.Level; //update the passive item level in the inventory manager
        }
        
    }
}
