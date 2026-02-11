using UnityEngine;


[CreateAssetMenu (fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]

public class WeaponScriptableObject : ScriptableObject
{
    public GameObject prefab;
    //base stats for weapons 
    public float damage;
    public float speed;
    public float CooldownDuration;
    public int pierce;
}
