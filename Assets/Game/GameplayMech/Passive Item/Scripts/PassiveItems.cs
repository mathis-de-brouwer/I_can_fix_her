using UnityEngine;

public class PassiveItems : MonoBehaviour
{
    protected PlayerStats player; 
    public PassiveItemsScriptableObjects passiveItemsData;

    protected virtual void ApplyModifier()
    {
        //Apply the boost to the right stat in the child classes    
    }

    void Start()
    {
        player = FindAnyObjectByType<PlayerStats> ();
        ApplyModifier();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
