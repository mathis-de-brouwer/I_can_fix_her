using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    [Header("Hit Feedback")]
    public Color hitFlashColor = Color.red;
    public float hitFlashDuration = 0.1f;
    public float staggerDuration = 0.15f;
    public float knockbackForce = 4f;

    // current stats
    float currentMoveSpeed;
    float currentHealth;
    float currentDamage;

    bool isStaggered = false;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Color originalColor;

    private void Awake()
    {
        currentMoveSpeed = enemyData.moveSpeed;
        currentDamage = enemyData.damage;
        currentHealth = enemyData.maxHealth;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public float GetMoveSpeed() => currentMoveSpeed;

    public void TakeDamage(float dmg, Vector2 hitDirection = default)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Kill();
            return;
        }

        if (sr != null)
            StartCoroutine(FlashColor());

        if (rb != null)
            StartCoroutine(Stagger(hitDirection));
    }

    IEnumerator FlashColor()
    {
        sr.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        sr.color = originalColor;
    }

    IEnumerator Stagger(Vector2 hitDirection)
    {
        isStaggered = true;

        if (hitDirection != Vector2.zero)
            rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(staggerDuration);
        isStaggered = false;
    }

    public bool IsStaggered() => isStaggered;

    public void Kill()
    {
        DropRateManager dropManager = GetComponent<DropRateManager>();
        if (dropManager != null)
            dropManager.TriggerDrop();

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (isStaggered) return;

        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }
}
