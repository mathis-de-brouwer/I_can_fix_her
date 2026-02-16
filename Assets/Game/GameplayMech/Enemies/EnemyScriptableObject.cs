using UnityEngine;



[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject 
{
    public float maxHealth;
    public float moveSpeed;
    public float damage;
    
}
