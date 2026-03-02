using UnityEngine;

public class PickUp : MonoBehaviour
{
    [Header("Attraction")]
    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private float maxFollowSpeed = 20f;

    private Transform _followTarget;
    private Rigidbody2D _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void BeginFollow(Transform target)
    {
        _followTarget = target;

        if (_rb != null)
        {
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_followTarget == null)
            return;

        Vector2 currentPosition = _rb != null ? _rb.position : (Vector2)transform.position;
        Vector2 targetPosition = _followTarget.position;
        Vector2 toTarget = targetPosition - currentPosition;

        Vector2 desiredVelocity = toTarget.normalized * followSpeed;
        if (desiredVelocity.magnitude > maxFollowSpeed)
            desiredVelocity = desiredVelocity.normalized * maxFollowSpeed;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, desiredVelocity, 0.25f);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        if (TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect();
        }

        Destroy(gameObject);
    }
}
