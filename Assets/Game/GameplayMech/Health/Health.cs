using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [SerializeField] private int startingHealth = 4;
    private int currentHealth;

    public int CurrentHealth => currentHealth;

    public int MaxHealth => startingHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, startingHealth);

    }

    private void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }

}
