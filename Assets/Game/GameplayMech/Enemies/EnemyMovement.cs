using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    Transform _player;
    Rigidbody2D _rb;
    EnemyStats _stats;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<EnemyStats>();
    }

    void Start()
    {
        Playermovement playerMovement = FindAnyObjectByType<Playermovement>();
        if (playerMovement != null)
            _player = playerMovement.transform;
        else
            Debug.LogWarning("EnemyMovement: No Playermovement found in scene.");
    }

    void FixedUpdate()
    {
        if (_player == null || _stats.IsStaggered()) return;

        Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
        _rb.linearVelocity = dir * _stats.GetMoveSpeed();
    }
}
