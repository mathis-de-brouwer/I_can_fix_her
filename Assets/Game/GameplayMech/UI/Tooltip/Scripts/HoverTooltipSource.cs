using UnityEngine;
using UnityEngine.EventSystems;

public sealed class HoverTooltipSource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IMoveHandler, ISelectHandler, IDeselectHandler
{
    [Header("Data (optional override)")]
    [SerializeField] private Sprite icon;
    [SerializeField] private string title;
    [TextArea(2, 6)]
    [SerializeField] private string body;

    [Header("Behavior")]
    [SerializeField] private bool showOnPointerHover = true;
    [SerializeField] private bool showOnSelect = false;
    [SerializeField] private bool showOnRightClick = false;
    [SerializeField] private bool rightClickToggles = true;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    bool _rightClickVisible;

    public void SetContent(Sprite newIcon, string newTitle, string newBody)
    {
        icon = newIcon;
        title = newTitle;
        body = newBody;

        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: SetContent(title='{title}', bodyLen={(body ?? string.Empty).Length})", this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: OnPointerEnter (hoverEnabled={showOnPointerHover})", this);

        if (!showOnPointerHover)
            return;

        Show(eventData.position, "PointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: OnPointerExit (hoverEnabled={showOnPointerHover})", this);

        if (!showOnPointerHover)
            return;

        TooltipController.Instance?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: OnPointerClick button={eventData.button} (rightClickEnabled={showOnRightClick})", this);

        if (!showOnRightClick)
            return;

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (rightClickToggles)
        {
            _rightClickVisible = !_rightClickVisible;

            if (debugLogs)
                Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: RightClick toggle -> visible={_rightClickVisible}", this);

            if (!_rightClickVisible)
            {
                TooltipController.Instance?.Hide();
                return;
            }
        }

        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: About to Show. controller={(TooltipController.Instance != null ? TooltipController.Instance.name : "null")} title='{title}' bodyLen={(body ?? string.Empty).Length}", this);

        Show(eventData.position, "RightClick");
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: OnSelect (showOnSelect={showOnSelect})", this);

        if (!showOnSelect)
            return;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Show(screenPos, "Select");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: OnDeselect (showOnSelect={showOnSelect})", this);

        if (!showOnSelect)
            return;

        TooltipController.Instance?.Hide();
    }

    public void OnMove(AxisEventData eventData)
    {
        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: OnMove (showOnSelect={showOnSelect})", this);

        if (!showOnSelect)
            return;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Show(screenPos, "Move");
    }

    void Show(Vector2 screenPos, string reason)
    {
        if (TooltipController.Instance == null)
        {
            if (debugLogs)
                Debug.LogWarning($"[Tooltip] {name}#{GetInstanceID()}: Show skipped ({reason}) - TooltipController.Instance is null", this);
            return;
        }

        if (debugLogs)
            Debug.Log($"[Tooltip] {name}#{GetInstanceID()}: Show ({reason}) pos={screenPos} title='{title}' bodyLen={(body ?? string.Empty).Length}", this);

        TooltipController.Instance.Show(icon, title, body, screenPos);
    }
}