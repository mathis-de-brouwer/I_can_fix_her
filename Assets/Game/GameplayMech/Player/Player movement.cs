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
    float lastHorizontalDirection = 1f;

    public InputAction playerControlls;

    [Header("Dash")]
    public InputAction dashAction;
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public float dashInvincibilityDuration = 0.3f;

    public bool InputLocked { get; set; }

    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;
    Vector2 dashDirection;

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
    public PlayerStats playerStats;

    void Update()
    {
        if (InputLocked)
        {
            movementDirection = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        movementDirection = playerControlls.ReadValue<Vector2>();

        if(movementDirection != Vector2.zero)
            lastMoveDirection = movementDirection;

        if(movementDirection.x != 0)
            lastHorizontalDirection = Mathf.Sign(movementDirection.x);

        animator.SetFloat("Speed", movementDirection.sqrMagnitude);

        if(!isDashing && spriteRenderer != null && movementDirection != Vector2.zero)
            spriteRenderer.flipX = lastHorizontalDirection < 0;

        if(dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if(isDashing)
        {
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0)
            {
                isDashing = false;
                Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
            }
        }

        if(dashAction.WasPressedThisFrame() && dashCooldownTimer <= 0 && !isDashing)
            StartDash();
    }

    void FixedUpdate()
    {
        if (InputLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

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

        dashDirection = (movementDirection != Vector2.zero) ? movementDirection.normalized : lastMoveDirection.normalized;

        if(spriteRenderer != null)
        {
            float horizontalSign = dashDirection.x != 0 ? Mathf.Sign(dashDirection.x) : lastHorizontalDirection;
            spriteRenderer.flipX = horizontalSign < 0;
        }

        if(playerStats != null)
            playerStats.StartInvincibility(dashInvincibilityDuration);

        animator.SetTrigger("Dash");
    }
}
