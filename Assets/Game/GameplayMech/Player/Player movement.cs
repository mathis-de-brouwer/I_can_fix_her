using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D rb;
    public float movementSpeed = 5f;
    public Animator animator;


    Vector2 movementDirection = Vector2.zero;

    public Vector2 lastMoveDirection = new Vector2(1, 0);

    public InputAction playerControlls;

    void OnEnable()
    {
        playerControlls.Enable();
    }

    void OnDisable()
    {
        playerControlls.Disable();
    }

    public CharacterSciptableObject characterData;

    // New: optional reference to PlayerStats so movement uses the runtime modified speed
    public PlayerStats playerStats;

    // Update is called once per frame
    void Update()
    {
        movementDirection = playerControlls.ReadValue<Vector2>();

        if(movementDirection != Vector2.zero)
        {
            lastMoveDirection = movementDirection;
        }

        animator.SetFloat("Horizontal", movementDirection.x);
        animator.SetFloat("Vertical", movementDirection.y);
        animator.SetFloat("Speed", movementDirection.sqrMagnitude);
    }

    void FixedUpdate()
    {
        // Use runtime playerStats.currentMovementSpeed if assigned (so passive items/bonuses apply).
        float speed = (playerStats != null) ? playerStats.currentMovementSpeed : characterData.MoveSpeed;

        // Use Rigidbody2D.linearVelocity to match project's Unity API
        rb.linearVelocity = new Vector2(movementDirection.x * speed, movementDirection.y * speed);
    }
}
