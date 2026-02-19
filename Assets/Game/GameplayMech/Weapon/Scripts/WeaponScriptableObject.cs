using UnityEngine;


[CreateAssetMenu (fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]

public class WeaponScriptableObject : ScriptableObject
{
[SerializeField]
GameObject prefab;
public GameObject Prefab { get => prefab; private set => prefab = value; }

//Base stats for weapons
[SerializeField]
float damage;
public float Damage { get => damage; private set => damage = value; }

[SerializeField]
float speed;
public float Speed { get => speed; private set => speed = value; }

[SerializeField]
float cooldownDuration;
public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

[SerializeField]
int pierce;
public int Pierce { get => pierce; private set => pierce = value; }
[SerializeField]
int level; //not meant to be modified in the game (inspector only)
public int Level { get => level; private set => level = value; }

[SerializeField]
 GameObject nextLevelPrefab; // the prefab of the next level i.e what the weapon becomes at the next level. Not to be confused with the prefab to be spawned at the next level.
public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }
}
