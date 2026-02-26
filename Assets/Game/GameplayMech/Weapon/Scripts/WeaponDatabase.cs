using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "ScriptableObjects/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public List<GameObject> weaponPrefabs = new List<GameObject>();
}