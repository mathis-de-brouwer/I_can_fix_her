using UnityEngine;

public class OrbitBehavior : ProjectileWeaponBehavior
{
    [Header("Orbit Settings")]
    public float orbitRadius = 2f;
    public float orbitSpeed = 200f; // degrees per second

    private Transform _player;
    private float _currentAngle;
    private float _angleOffset;

    public void InitializeOrbit(int index, int totalCount)
    {
        int safeTotal = Mathf.Max(1, totalCount);
        int safeIndex = Mathf.Clamp(index, 0, safeTotal - 1);

        _angleOffset = (360f / safeTotal) * safeIndex;
    }

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

    private void Update()
    {
        if (_player == null) return;

        _currentAngle += orbitSpeed * Time.deltaTime;
        if (_currentAngle >= 360f) _currentAngle -= 360f;

        float finalAngle = _currentAngle + _angleOffset;

        float rad = finalAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;
        transform.position = _player.position + offset;

        transform.rotation = Quaternion.Euler(0f, 0f, finalAngle);
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