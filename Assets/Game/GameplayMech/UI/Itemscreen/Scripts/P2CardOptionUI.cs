using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Single selectable card slot inside <see cref="P2CardChoiceUI"/>.</summary>
public class P2CardOptionUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button button;

    [SerializeField] private float disabledAlpha = 0.45f;

    P2Card _card;
    Action<P2Card> _onClicked;

    public P2Card Card => _card;

    public void Setup(P2Card card, Action<P2Card> onClicked)
    {
        _card = card;
        _onClicked = onClicked;

        gameObject.SetActive(true);
        SetDimmed(false);

        if (icon != null)
            icon.sprite = card.effect != null && card.effect.CardArtOverride != null
                ? card.effect.CardArtOverride
                : card.icon;

        if (nameText != null)
            nameText.text = card.cardName;

        if (costText != null)
            costText.text = card.cost.ToString();

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
        _onClicked?.Invoke(_card);
    }
}