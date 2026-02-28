using UnityEngine;

public class OrbitBehavior : ProjectileWeaponBehavior
{
    [Header("Orbit Settings")]
    public float orbitRadius = 2f;
    public float orbitSpeed = 200f; // degrees per second

    private Transform _player;
    private float _currentAngle;

    protected override void Start()
    {
        // Do NOT call base.Start() — we don't want the auto-destroy timer.
        // The controller will manage our lifetime via cooldown cycling.

        Playermovement playerMovement = FindAnyObjectByType<Playermovement>();
        if (playerMovement != null)
            _player = playerMovement.transform;
        else
            Debug.LogWarning("OrbitBehavior: No player found in scene.");

        _currentAngle = 0f;
    }

    void Update()
    {
        if (_player == null) return;

        _currentAngle += orbitSpeed * Time.deltaTime;
        if (_currentAngle >= 360f) _currentAngle -= 360f;

        float rad = _currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;
        transform.position = _player.position + offset;

        // Optional: rotate the sprite to face the orbit direction
        transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        // Damage enemies but do NOT reduce pierce — the orbit weapon hits continuously
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                Vector2 hitDir = (col.transform.position - transform.position).normalized;
                enemy.TakeDamage(GetCurrentDamage(), hitDir);
            }
        }
    }
}