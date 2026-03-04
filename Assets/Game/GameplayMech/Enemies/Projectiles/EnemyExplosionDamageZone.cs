using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyExplosionDamageZone : MonoBehaviour
{
    [SerializeField, Min(0f)] float lifetimeSeconds = 0.35f;
    [SerializeField] bool damagePlayerOnce = true;

    float _damage;
    bool _damagedPlayer;

    CircleCollider2D _circle;

    void Awake()
    {
        _circle = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetimeSeconds);
    }

    public void Initialize(float damage, float radius)
    {
        _damage = Mathf.Max(0f, damage);

        if (_circle != null && radius > 0f)
            _circle.radius = radius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    void TryDamage(Collider2D other)
    {
        if (_damage <= 0f)
            return;

        if (damagePlayerOnce && _damagedPlayer)
            return;

        PlayerStats player = other != null ? other.GetComponentInParent<PlayerStats>() : null;
        if (player == null)
            return;

        player.TakeDamage(_damage);

        _damagedPlayer = true;
    }
}