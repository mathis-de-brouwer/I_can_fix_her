using UnityEngine;

public sealed class EnemyFireballBehavior : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] float speed = 8f;
    [SerializeField, Min(0f)] float destroyAfterSeconds = 3f;

    [Header("Damage")]
    [SerializeField, Min(0f)] float damage = 10f;

    [Header("Explosion")]
    [SerializeField, Min(0f)] float explosionRadius = 1.75f;

    [Header("Explosion Prefab (does damage via collider)")]
    [SerializeField] GameObject explosionVfxPrefab;

    Vector2 _direction = Vector2.right;
    bool _exploded;

    void Start()
    {
        if (destroyAfterSeconds > 0f)
            Destroy(gameObject, destroyAfterSeconds);
    }

    void Update()
    {
        transform.position += (Vector3)(_direction * speed * Time.deltaTime);
    }

    public void Initialize(Vector2 direction)
    {
        _direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        transform.rotation = DirectionToRotation(_direction);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (_exploded || col == null)
            return;

        if (col.GetComponentInParent<PlayerStats>() != null)
            Explode();
    }

    void Explode()
    {
        if (_exploded)
            return;

        _exploded = true;

        if (explosionVfxPrefab != null)
        {
            Quaternion rotation = DirectionToRotation(_direction);
            GameObject vfx = Instantiate(explosionVfxPrefab, transform.position, rotation);

            if (vfx.TryGetComponent(out EnemyExplosionDamageZone damageZone))
                damageZone.Initialize(damage, explosionRadius);
        }

        Destroy(gameObject);
    }

    static Quaternion DirectionToRotation(Vector2 dir)
    {
        if (dir.sqrMagnitude <= 0.0001f)
            return Quaternion.identity;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle);
    }
}