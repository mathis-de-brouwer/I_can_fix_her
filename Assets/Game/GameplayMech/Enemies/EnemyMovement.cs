using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Transform player;
    public float moveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Playermovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed*Time.deltaTime); // make the enemy constantly move towards to the player
    }
}
