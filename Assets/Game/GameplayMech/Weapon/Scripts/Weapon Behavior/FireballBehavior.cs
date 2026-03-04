using UnityEngine;

public class FireballBehavior : ProjectileWeaponBehavior
{
    [Header("Explosion")]
    [SerializeField] private bool explodeOnLifetimeEnd = true;
    [SerializeField, Min(0f)] private float explosionRadius = 1.75f;

    [Header("Explosion Prefab (does damage via collider)")]
    [SerializeField] private GameObject explosionVfxPrefab;

    private bool _exploded;

    protected override void Start()
    {
        if (explodeOnLifetimeEnd && destroyAfterSeconds > 0f)
            Invoke(nameof(Explode), destroyAfterSeconds);
    }

    private void Update()
    {
        transform.position += direction * weaponData.Speed * Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (_exploded || col == null)
            return;

        if (col.GetComponentInParent<EnemyStats>() != null)
            Explode();
    }

    private void Explode()
    {
        if (_exploded)
            return;

        _exploded = true;

        UiSfx.PlayFireballExplode();

        if (explosionVfxPrefab != null)
        {
            Quaternion rotation = DirectionToRotation(direction);
            GameObject vfx = Instantiate(explosionVfxPrefab, transform.position, rotation);

            if (vfx.TryGetComponent(out ExplosionDamageZone damageZone))
                damageZone.Initialize(GetCurrentDamage(), explosionRadius);
        }

        Destroy(gameObject);
    }

    private static Quaternion DirectionToRotation(Vector2 dir)
    {
        if (dir.sqrMagnitude <= 0.0001f)
            return Quaternion.identity;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle);
    }

    public void Initialize(Vector2 dir, float radius)
    {
        Vector2 d = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        direction = d;
        transform.rotation = DirectionToRotation(d);

        explosionRadius = Mathf.Max(0f, radius);

        UiSfx.PlayFireballCast();
    }
}