using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamageZone : MonoBehaviour
{
    [SerializeField, Min(0f)] private float lifetimeSeconds = 0.35f;
    [SerializeField] private bool damageEachEnemyOnce = true;

    private float _damage;
    private readonly HashSet<EnemyStats> _damagedEnemies = new HashSet<EnemyStats>();

    private CircleCollider2D _circle;

    private void Awake()
    {
        _circle = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetimeSeconds);
    }

    public void Initialize(float damage, float radius)
    {
        _damage = damage;

        if (_circle != null && radius > 0f)
            _circle.radius = radius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (_damage <= 0f)
            return;

        EnemyStats enemy = other != null ? other.GetComponentInParent<EnemyStats>() : null;
        if (enemy == null)
            return;

        if (damageEachEnemyOnce && _damagedEnemies.Contains(enemy))
            return;

        Vector2 hitDir = (enemy.transform.position - transform.position).normalized;
        enemy.TakeDamage(_damage, hitDir);

        if (damageEachEnemyOnce)
            _damagedEnemies.Add(enemy);
    }
}