using UnityEngine;

public class MagicStaffController : WeaponController
{
    [Header("Staff Visual")]
    [SerializeField] private GameObject staffVisualPrefab;
    [SerializeField] private Vector2 localOffset = new Vector2(0.8f, 0.2f);
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobFrequency = 2.5f;

    [Header("Fire Direction")]
    [SerializeField] private bool includeDiagonals = false;

    [Header("Projectile Spawn")]
    [SerializeField, Min(0f)] private float spawnDistance = 0.8f;

    [Header("Explosion Radius Scaling")]
    [SerializeField, Min(0f)] private float baseExplosionRadius = 1.75f;
    [SerializeField, Min(0f)] private float explosionRadiusPerLevel = 0.25f;

    private Transform _player;
    private Transform _staffVisualTransform;
    private float _bobTime;

    protected override void Start()
    {
        base.Start();

        _player = transform.parent != null ? transform.parent : transform;
        SpawnStaffVisual();
    }

    private void SpawnStaffVisual()
    {
        if (staffVisualPrefab == null || _player == null)
            return;

        GameObject instance = Instantiate(staffVisualPrefab, _player);
        instance.transform.localPosition = localOffset;
        _staffVisualTransform = instance.transform;
    }

    protected override void Update()
    {
        base.Update();
        UpdateStaffVisualTransform();
    }

    private void UpdateStaffVisualTransform()
    {
        if (_staffVisualTransform == null)
            return;

        _bobTime += Time.deltaTime;
        float bob = Mathf.Sin(_bobTime * bobFrequency) * bobAmplitude;

        _staffVisualTransform.localPosition = new Vector3(localOffset.x, localOffset.y + bob, 0f);
        _staffVisualTransform.localRotation = Quaternion.identity;
    }

    protected override void Attack()
    {
        base.Attack();

        if (weaponData == null || weaponData.Prefab == null)
            return;

        int level = Mathf.Max(1, weaponData.Level);
        float radius = baseExplosionRadius + (explosionRadiusPerLevel * (level - 1));

        Vector2 dir = PickRandomDirection(includeDiagonals);
        Vector2 dirNormalized = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;

        Vector3 origin = _staffVisualTransform != null ? _staffVisualTransform.position : transform.position;
        Vector3 spawnPos = origin + (Vector3)(dirNormalized * spawnDistance);

        GameObject projectile = Instantiate(weaponData.Prefab);
        projectile.transform.position = spawnPos;

        if (projectile.TryGetComponent(out FireballBehavior fireball))
            fireball.Initialize(dirNormalized, radius);
        else if (projectile.TryGetComponent(out ProjectileWeaponBehavior projectileBehavior))
            projectileBehavior.DirectionChecker(dirNormalized);
    }

    private static Vector2 PickRandomDirection(bool diagonals)
    {
        if (!diagonals)
        {
            switch (Random.Range(0, 4))
            {
                case 0: return Vector2.right;
                case 1: return Vector2.left;
                case 2: return Vector2.up;
                default: return Vector2.down;
            }
        }

        switch (Random.Range(0, 8))
        {
            case 0: return Vector2.right;
            case 1: return Vector2.left;
            case 2: return Vector2.up;
            case 3: return Vector2.down;
            case 4: return new Vector2(1f, 1f).normalized;
            case 5: return new Vector2(1f, -1f).normalized;
            case 6: return new Vector2(-1f, 1f).normalized;
            default: return new Vector2(-1f, -1f).normalized;
        }
    }
}