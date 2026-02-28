using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TooltipUI : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private RectTransform root;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    [Header("Layout")]
    [SerializeField] private Vector2 padding = new Vector2(12f, 12f);

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    CanvasGroup _canvasGroup;

    public RectTransform Root => root != null ? root : transform as RectTransform;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Hide();
    }

    public void Show(Sprite sprite, string title, string body)
    {
        if (debugLogs)
            Debug.Log($"[TooltipUI] Show title='{title}' bodyLen={(body ?? string.Empty).Length}", this);

        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }

        if (titleText != null)
            titleText.text = title ?? string.Empty;

        if (bodyText != null)
            bodyText.text = body ?? string.Empty;

        SetVisible(true);
    }

    public void Hide()
    {
        if (debugLogs)
            Debug.Log("[TooltipUI] Hide", this);

        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        if (_canvasGroup == null)
            return;

        _canvasGroup.alpha = visible ? 1f : 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetScreenPositionClamped(Vector2 screenPos, Canvas canvas)
    {
        RectTransform rt = Root;
        if (rt == null || canvas == null)
            return;

        RectTransform canvasRt = canvas.transform as RectTransform;
        if (canvasRt == null)
            return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRt,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint);

        rt.anchoredPosition = localPoint;

        ClampToCanvas(rt, canvasRt);
    }

    void ClampToCanvas(RectTransform tooltip, RectTransform canvas)
    {
        Vector2 canvasSize = canvas.rect.size;
        Vector2 tooltipSize = tooltip.rect.size;

        Vector2 pos = tooltip.anchoredPosition;

        float halfCanvasX = canvasSize.x * 0.5f;
        float halfCanvasY = canvasSize.y * 0.5f;

        float halfTipX = tooltipSize.x * 0.5f;
        float halfTipY = tooltipSize.y * 0.5f;

        float minX = -halfCanvasX + halfTipX + padding.x;
        float maxX = halfCanvasX - halfTipX - padding.x;
        float minY = -halfCanvasY + halfTipY + padding.y;
        float maxY = halfCanvasY - halfTipY - padding.y;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        tooltip.anchoredPosition = pos;
    }
}