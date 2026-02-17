using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        //checks if the game object has the Icollectible interface
        if(col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            //If it does, call the collect method
            collectible.Collect();
        }
    }
}
