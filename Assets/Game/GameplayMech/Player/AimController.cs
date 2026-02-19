using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [Header("Aim Input - Stick")]
    public InputAction aimAction;           // Right stick (Gamepad)

    [Header("Aim Input - Keyboard")]
    public InputAction aimUp;               // Default: I
    public InputAction aimDown;             // Default: K
    public InputAction aimLeft;             // Default: J
    public InputAction aimRight;            // Default: L

    [Header("References")]
    public Playermovement playerMovement;   // Drag the player here, or auto-resolve in Start

    // The resolved aim direction — read by WeaponController
    public Vector2 AimDirection { get; private set; }

    bool _hasAimInput;

    void Start()
    {
        if(playerMovement == null)
            playerMovement = GetComponent<Playermovement>();
    }

    void OnEnable()
    {
        aimAction.Enable();
        aimUp.Enable();
        aimDown.Enable();
        aimLeft.Enable();
        aimRight.Enable();
    }

    void OnDisable()
    {
        aimAction.Disable();
        aimUp.Disable();
        aimDown.Disable();
        aimLeft.Disable();
        aimRight.Disable();
    }

    void Update()
    {
        Vector2 raw = aimAction.ReadValue<Vector2>();
        _hasAimInput = raw.sqrMagnitude > 0.25f; // dead-zone

        // Keyboard aim composite — only checked when stick has no input
        if(!_hasAimInput)
        {
            Vector2 keyboardAim = Vector2.zero;

            if(aimUp.IsPressed())    keyboardAim.y += 1f;
            if(aimDown.IsPressed())  keyboardAim.y -= 1f;
            if(aimRight.IsPressed()) keyboardAim.x += 1f;
            if(aimLeft.IsPressed())  keyboardAim.x -= 1f;

            if(keyboardAim.sqrMagnitude > 0f)
            {
                raw = keyboardAim.normalized;
                _hasAimInput = true;
            }
        }

        // Use explicit aim when available, otherwise fall back to last move direction
        AimDirection = _hasAimInput ? raw.normalized : playerMovement.lastMoveDirection.normalized;
    }
}