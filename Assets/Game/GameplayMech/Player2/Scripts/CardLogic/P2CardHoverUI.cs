using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class P2CardHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Motion")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float liftY = 30f;
    [SerializeField] private float animDuration = 0.08f;

    private RectTransform _rect;
    private Vector3 _baseScale;
    private Vector2 _baseAnchoredPos;
    private Coroutine _anim;
    private Canvas _hoverCanvas;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        if (_rect != null)
        {
            _baseScale = _rect.localScale;
            _baseAnchoredPos = _rect.anchoredPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnsureHoverCanvasExists();
        if (_hoverCanvas != null)
            _hoverCanvas.overrideSorting = true;

        AnimateTo(_baseAnchoredPos + new Vector2(0f, liftY), _baseScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverCanvas != null)
            _hoverCanvas.overrideSorting = false;

        AnimateTo(_baseAnchoredPos, _baseScale);
    }

    private void EnsureHoverCanvasExists()
    {
        if (_hoverCanvas != null)
            return;

        _hoverCanvas = GetComponent<Canvas>();
        if (_hoverCanvas == null)
            _hoverCanvas = gameObject.AddComponent<Canvas>();

        var scaler = GetComponent<UnityEngine.UI.CanvasScaler>();
        if (scaler != null)
            Destroy(scaler);

        var raycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster != null)
            Destroy(raycaster);

        _hoverCanvas.sortingOrder = 1000;
    }

    private void AnimateTo(Vector2 targetPos, Vector3 targetScale)
    {
        if (_rect == null)
            return;

        if (_anim != null)
            StopCoroutine(_anim);

        _anim = StartCoroutine(AnimateRoutine(targetPos, targetScale));
    }

    private IEnumerator AnimateRoutine(Vector2 targetPos, Vector3 targetScale)
    {
        Vector2 startPos = _rect.anchoredPosition;
        Vector3 startScale = _rect.localScale;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = animDuration <= 0f ? 1f : Mathf.Clamp01(t / animDuration);

            _rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, a);
            _rect.localScale = Vector3.Lerp(startScale, targetScale, a);

            yield return null;
        }

        _rect.anchoredPosition = targetPos;
        _rect.localScale = targetScale;
        _anim = null;
    }
}