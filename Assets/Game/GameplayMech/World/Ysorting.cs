using UnityEngine;

public class Ysorting : MonoBehaviour
{
    [Tooltip("Vertical offset in world units from this object's pivot used as the sort origin. " +
             "Use a negative value to push the sort point toward the base of tall sprites.")]
    public float sortingOriginOffset = 0f;

    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        _sr.sortingOrder = Mathf.RoundToInt(-(transform.position.y + sortingOriginOffset) * 100);
    }
}
