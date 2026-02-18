using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
PlayerStats player;
CircleCollider2D playerCollector; 

public float pullSpeed;

    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.currentMagnet; 
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //checks if the game object has the Icollectible interface
        if(col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            //pulling animation for objects to the player 
            //this here gets the rigidbody2D component on the item 
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            //vector2 pointing from the item to the player
            Vector2 forceDirection = ( transform.position - col.transform.position).normalized;
            //Applies force to the item in the forceDirection with pullSpeed
            rb.AddForce(forceDirection * pullSpeed);

            //If it does, call the collect method
            collectible.Collect();
        }
    }
}
