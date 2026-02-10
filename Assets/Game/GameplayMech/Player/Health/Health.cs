using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [SerializeField] private int startingHealth = 4;
    private int currentHealth;
    private bool Dead;

    public int CurrentHealth => currentHealth;

    public int MaxHealth => startingHealth;

    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else
        {
            if (!Dead)
            {
                anim.SetTrigger("Die");
                GetComponent<Playermovement>().enabled = false;

                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                Dead = true;
            }
            
        }

    }

    private void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }

}
