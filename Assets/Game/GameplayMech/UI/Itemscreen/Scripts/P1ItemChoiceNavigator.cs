using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(P1ItemChoiceUI))]
public class P1ItemChoiceNavigator : MonoBehaviour
{
    [Header("Input Actions")]
    public InputAction navigateAction; // float (AxisComposite) or Vector2 (Stick/Dpad)
    public InputAction confirmAction;
    public InputAction tooltipAction;

    [Header("Navigation")]
    [SerializeField] private float navigateThreshold = 0.5f;

    P1ItemChoiceUI _choiceUI;

    int _selectedActiveIndex;
    bool _active;
    bool _navigateHeld;

    bool _tooltipVisible;

    void Awake()
    {
        _choiceUI = GetComponent<P1ItemChoiceUI>();
    }

    void OnEnable()
    {
        navigateAction.Enable();
        confirmAction.Enable();
        tooltipAction.Enable();

        confirmAction.performed += OnConfirmPerformed;
        tooltipAction.performed += OnTooltipPerformed;
    }

    void OnDisable()
    {
        confirmAction.performed -= OnConfirmPerformed;
        tooltipAction.performed -= OnTooltipPerformed;

        navigateAction.Disable();
        confirmAction.Disable();
        tooltipAction.Disable();
    }

    public void Activate()
    {
        _active = true;
        _selectedActiveIndex = 0;
        _navigateHeld = false;
        _tooltipVisible = false;

        HideP1Tooltip();
        RefreshSelection();
    }

    public void Deactivate()
    {
        _active = false;
        _tooltipVisible = false;

        HideP1Tooltip();
        ClearSelection();
    }

    void Update()
    {
        if (!_active) return;

        int activeCount = ActiveSlotCount();
        if (activeCount == 0) return;

        float x = ReadNavigateX();

        if (Mathf.Abs(x) >= navigateThreshold)
        {
            if (!_navigateHeld)
            {
                _navigateHeld = true;

                _selectedActiveIndex += x > 0f ? 1 : -1;
                _selectedActiveIndex = WrapIndex(_selectedActiveIndex, activeCount);

                RefreshSelection();
                RefreshTooltipIfVisible();
            }
        }
        else
        {
            _navigateHeld = false;
        }
    }

    void OnConfirmPerformed(InputAction.CallbackContext context)
    {
        if (!_active) return;

        P1ItemOptionUI slot = GetActiveSlot(_selectedActiveIndex);
        slot?.SimulateClick();
    }

    void OnTooltipPerformed(InputAction.CallbackContext context)
    {
        if (!_active) return;

        _tooltipVisible = !_tooltipVisible;

        if (!_tooltipVisible)
        {
            HideP1Tooltip();
            return;
        }

        ShowTooltipForSelected();
    }

    void RefreshTooltipIfVisible()
    {
        if (!_tooltipVisible)
            return;

        ShowTooltipForSelected();
    }

    void ShowTooltipForSelected()
    {
        P1ItemOptionUI slot = GetActiveSlot(_selectedActiveIndex);
        if (slot == null)
            return;

        if (!slot.TryBuildTooltip(out Sprite icon, out string title, out string body))
            return;

        TooltipController controller = slot.TooltipController != null ? slot.TooltipController : TooltipController.Instance;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, slot.transform.position);
        controller?.Show(icon, title, body, screenPos);
    }

    void HideP1Tooltip()
    {
        // Hide on the P1 channel if we can find it; otherwise fall back to global.
        // We intentionally do NOT hide “every tooltip in the game”.

        P1ItemOptionUI slot = GetActiveSlot(_selectedActiveIndex);
        TooltipController controller = slot != null && slot.TooltipController != null ? slot.TooltipController : TooltipController.Instance;
        controller?.Hide();
    }

    static int WrapIndex(int index, int count)
    {
        if (count <= 0) return 0;

        index %= count;
        if (index < 0) index += count;
        return index;
    }

    float ReadNavigateX()
    {
        if (navigateAction.activeControl != null && navigateAction.activeControl.valueType == typeof(UnityEngine.Vector2))
            return navigateAction.ReadValue<UnityEngine.Vector2>().x;

        return navigateAction.ReadValue<float>();
    }

    int ActiveSlotCount()
    {
        if (_choiceUI == null || _choiceUI.OptionSlots == null) return 0;

        int count = 0;
        foreach (P1ItemOptionUI slot in _choiceUI.OptionSlots)
            if (slot != null && slot.gameObject.activeSelf) count++;

        return count;
    }

    P1ItemOptionUI GetActiveSlot(int activeIndex)
    {
        if (_choiceUI == null || _choiceUI.OptionSlots == null) return null;

        int current = 0;
        foreach (P1ItemOptionUI slot in _choiceUI.OptionSlots)
        {
            if (slot == null || !slot.gameObject.activeSelf) continue;

            if (current == activeIndex)
                return slot;

            current++;
        }

        return null;
    }

    void RefreshSelection()
    {
        if (_choiceUI == null || _choiceUI.OptionSlots == null) return;

        int current = 0;
        foreach (P1ItemOptionUI slot in _choiceUI.OptionSlots)
        {
            if (slot == null) continue;

            if (!slot.gameObject.activeSelf)
            {
                slot.SetSelected(false);
                continue;
            }

            slot.SetSelected(current == _selectedActiveIndex);
            current++;
        }
    }

    void ClearSelection()
    {
        if (_choiceUI == null || _choiceUI.OptionSlots == null) return;

        foreach (P1ItemOptionUI slot in _choiceUI.OptionSlots)
            slot?.SetSelected(false);
    }
}