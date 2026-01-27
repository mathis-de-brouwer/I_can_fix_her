using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public Rigidbody2D rb;
   public float movementSpeed = 5f;

   Vector2 movementDirection = Vector2.zero;

   public InputAction playerControlls;

    void OnEnable()
    {
        playerControlls.Enable();
    }

    void OnDisable()
    {
        playerControlls.Disable();
    }

    

    // Update is called once per frame
    void Update()
    {
        movementDirection = playerControlls.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2 (movementDirection.x * movementSpeed, movementDirection.y * movementSpeed);
    }
}
