using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    public EnemyScriptableObject enemyData;
    Transform player;

    void Start()
    {
        player = FindAnyObjectByType<Playermovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyData.moveSpeed*Time.deltaTime); // make the enemy constantly move towards to the player
    }
}
