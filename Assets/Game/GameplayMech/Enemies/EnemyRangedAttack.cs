using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public sealed class EnemyRangedAttack : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] EnemyFireballBehavior projectilePrefab;
    [SerializeField] Transform muzzle;

    [Header("Timing")]
    [SerializeField, Min(0.01f)] float cooldownSeconds = 1.25f;

    [Header("Range Gating")]
    [SerializeField, Min(0f)] float maxRange = 8f;
    [SerializeField, Min(0f)] float minRange = 0f;

    EnemyMovement _movement;
    Transform _player;
    float _cooldownTimer;

    void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        Playermovement playerMovement = FindAnyObjectByType<Playermovement>();
        _player = playerMovement != null ? playerMovement.transform : null;
    }

    void Update()
    {
        if (projectilePrefab == null || _movement == null || _player == null)
            return;

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            return;
        }

        float distance = Vector2.Distance(transform.position, _player.position);
        if (distance > maxRange || distance < minRange)
            return;

        Fire();
        _cooldownTimer = cooldownSeconds;
    }

    void Fire()
    {
        Vector3 spawnPos = muzzle != null ? muzzle.position : transform.position;
        Vector2 dir = _movement.AimDirection;

        EnemyFireballBehavior projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        projectile.Initialize(dir);
    }
}