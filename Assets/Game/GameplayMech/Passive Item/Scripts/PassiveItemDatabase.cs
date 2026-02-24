using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemDatabase", menuName = "ScriptableObjects/PassiveItemDatabase")]
public class PassiveItemDatabase : ScriptableObject
{
    [Tooltip("Drag passive item prefabs here — each must have a PassiveItems component.")]
    public List<GameObject> passiveItemPrefabs = new List<GameObject>();
}