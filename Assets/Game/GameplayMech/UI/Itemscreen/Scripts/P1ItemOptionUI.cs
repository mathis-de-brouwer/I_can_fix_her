using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P1ItemOptionUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private Button button;

    [SerializeField] private float disabledAlpha = 0.45f;

    [Header("Controller Selection Scale")]
    [SerializeField] private float selectedIconScale = 1.15f;

    GameObject _prefab;
    Action<GameObject> _onClicked;

    Vector3 _iconBaseScale;

    public GameObject Prefab => _prefab;

    void Awake()
    {
        if (icon != null)
            _iconBaseScale = icon.rectTransform.localScale;
    }

    public void Setup(GameObject prefab, Action<GameObject> onClicked)
    {
        _prefab = prefab;
        _onClicked = onClicked;

        gameObject.SetActive(true);
        SetDimmed(false);
        SetSelected(false);

        PassiveItems passive = prefab.GetComponent<PassiveItems>();
        PassiveItemsScriptableObjects data = passive != null ? passive.passiveItemsData : null;

        if (icon != null)
            icon.sprite = data != null ? data.Icon : null;

        if (nameText != null)
            nameText.text = data != null && !string.IsNullOrEmpty(data.ItemName) ? data.ItemName : prefab.name;

        if (multiplierText != null)
            multiplierText.text = data != null ? $"x{data.Multiplier}" : string.Empty;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    public void SetMouseInputEnabled(bool enabled)
    {
        // Disables pointer clicks. Controller can still trigger via SimulateClick().
        if (button != null)
            button.interactable = enabled;
    }

    public void SetDimmed(bool dimmed)
    {
        if (icon != null)
        {
            Color c = icon.color;
            c.a = dimmed ? disabledAlpha : 1f;
            icon.color = c;
        }
    }

    public void SetSelected(bool selected)
    {
        if (icon == null) return;

        Vector3 scale = _iconBaseScale;
        if (selected)
            scale *= selectedIconScale;

        icon.rectTransform.localScale = scale;
    }

    public void SimulateClick()
    {
        OnClick();
    }

    void OnClick()
    {
        _onClicked?.Invoke(_prefab);
    }
}