using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    private PlayerStats player;
    private CircleCollider2D playerCollector;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (playerCollector != null && player != null)
            playerCollector.radius = player.currentMagnet;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.TryGetComponent(out ICollectible _))
            return;

        if (col.gameObject.TryGetComponent(out PickUp pickUp))
        {
            pickUp.BeginFollow(transform);
        }
    }
}
