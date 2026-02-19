using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float movementSpeed = 5f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    Vector2 movementDirection = Vector2.zero;

    public Vector2 lastMoveDirection = new Vector2(1, 0);
    float lastHorizontalDirection = 1f; // 1 = right, -1 = left

    public InputAction playerControlls;

    [Header("Dash")]
    public InputAction dashAction;
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public float dashInvincibilityDuration = 0.3f;

    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;
    Vector2 dashDirection;

    // Resolved once in Awake — no Inspector fields needed
    int _playerLayer;
    int _enemyLayer;

    void Awake()
    {
        _playerLayer = gameObject.layer;
        _enemyLayer = LayerMask.NameToLayer("Ennemies");

        if(_enemyLayer == -1)
            Debug.LogError("Layer 'Ennemies' not found! Check spelling in Tags and Layers.");
    }

    void OnEnable()
    {
        playerControlls.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        playerControlls.Disable();
        dashAction.Disable();
    }

    public CharacterSciptableObject characterData;

    // Optional reference to PlayerStats so movement uses the runtime modified speed
    public PlayerStats playerStats;

    void Update()
    {
        movementDirection = playerControlls.ReadValue<Vector2>();

        if(movementDirection != Vector2.zero)
        {
            lastMoveDirection = movementDirection;
        }

        // Only update last horizontal when there is actual horizontal input
        if(movementDirection.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(movementDirection.x);
        }

        animator.SetFloat("Speed", movementDirection.sqrMagnitude);

        // Update facing — always based on lastHorizontalDirection
        if(!isDashing && spriteRenderer != null && movementDirection != Vector2.zero)
        {
            spriteRenderer.flipX = lastHorizontalDirection < 0;
        }

        // Tick cooldown
        if(dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Tick active dash
        if(isDashing)
        {
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0)
            {
                isDashing = false;
                Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
            }
        }

        // Dash input
        if(dashAction.WasPressedThisFrame() && dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }
    }

    void FixedUpdate()
    {
        if(isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }

        float speed = (playerStats != null) ? playerStats.currentMovementSpeed : characterData.MoveSpeed;
        rb.linearVelocity = new Vector2(movementDirection.x * speed, movementDirection.y * speed);
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, true);

        // Dash in the direction the player is moving, or facing if standing still
        dashDirection = (movementDirection != Vector2.zero) ? movementDirection.normalized : lastMoveDirection.normalized;

        // Flip sprite to match dash direction using lastHorizontalDirection as fallback
        if(spriteRenderer != null)
        {
            float horizontalSign = dashDirection.x != 0 ? Mathf.Sign(dashDirection.x) : lastHorizontalDirection;
            spriteRenderer.flipX = horizontalSign < 0;
        }

        // Grant I-frames for the dash window
        if(playerStats != null)
        {
            playerStats.StartInvincibility(dashInvincibilityDuration);
        }

        animator.SetTrigger("Dash");
    }
}
