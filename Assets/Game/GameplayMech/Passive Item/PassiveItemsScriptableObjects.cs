using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemsScriptableObjects", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemsScriptableObjects : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value;} 

   [SerializeField]
   int level; //not meant to be modified in the game (inspector only)
   public int Level { get => level; private set => level = value; }

   [SerializeField]
   GameObject nextLevelPrefab; // the prefab of the next level i.e what the weapon becomes at the next level. Not to be confused with the prefab to be spawned at the next level.
   public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }
}
