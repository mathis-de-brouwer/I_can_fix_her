using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemsScriptableObjects", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemsScriptableObjects : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value;} 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
