using UnityEngine;

public sealed class TooltipController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TooltipUI tooltip;

    [Header("Fixed Position (optional)")]
    [SerializeField] private RectTransform fixedAnchor;

    static TooltipController _instance;

    public static TooltipController Instance => _instance;

    void Awake()
    {
        if (_instance == null)
            _instance = this;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (tooltip == null)
            tooltip = GetComponentInChildren<TooltipUI>(true);

        Hide();
    }

    public void Show(Sprite icon, string title, string body, Vector2 screenPos)
    {
        if (tooltip == null)
            return;

        tooltip.Show(icon, title, body);

        if (canvas == null)
            return;

        if (fixedAnchor != null)
        {
            RectTransform tooltipRt = tooltip.Root;
            RectTransform canvasRt = canvas.transform as RectTransform;
            if (tooltipRt != null && canvasRt != null)
                tooltipRt.anchoredPosition = fixedAnchor.anchoredPosition;

            return;
        }

        tooltip.SetScreenPositionClamped(screenPos, canvas);
    }

    public void Hide()
    {
        if (tooltip != null)
            tooltip.Hide();
    }
}