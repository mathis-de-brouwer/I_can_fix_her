using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    //current stats

    float currentMoveSpeed;
    float currentHealth;
    float currentDamage;

    private void Awake()
    {
        currentMoveSpeed = enemyData.moveSpeed;
        currentDamage = enemyData.damage;
        currentHealth = enemyData.maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
            Kill();
    }

    public void Kill()
    {
        DropRateManager dropManager = GetComponent<DropRateManager>();
        if (dropManager != null)
            dropManager.TriggerDrop();

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }
}
