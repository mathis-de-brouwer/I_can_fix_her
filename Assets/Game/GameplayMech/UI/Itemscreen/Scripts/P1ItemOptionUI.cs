using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single selectable passive item slot inside <see cref="P1ItemChoiceUI"/>.
/// Reads display data from the prefab's <see cref="PassiveItems"/> component.
/// </summary>
public class P1ItemOptionUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private Button button;

    [SerializeField] private float disabledAlpha = 0.45f;

    GameObject _prefab;
    Action<GameObject> _onClicked;

    public GameObject Prefab => _prefab;

    public void Setup(GameObject prefab, Action<GameObject> onClicked)
    {
        _prefab = prefab;
        _onClicked = onClicked;

        gameObject.SetActive(true);
        SetDimmed(false);

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

    public void SetDimmed(bool dimmed)
    {
        if (icon != null)
        {
            Color c = icon.color;
            c.a = dimmed ? disabledAlpha : 1f;
            icon.color = c;
        }
    }

    void OnClick()
    {
        _onClicked?.Invoke(_prefab);
    }
}