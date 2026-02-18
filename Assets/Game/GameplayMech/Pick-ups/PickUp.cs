using UnityEngine;

public class PickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) //Destroys the item once it hits the player so no need for fancy calculation of the distance with the player blablabla
        {
            Destroy(gameObject);
        }
    }
}
