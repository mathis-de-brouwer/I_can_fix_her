using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class P2DeckBuilderCardTileUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float disabledAlpha = 0.45f;

    private Button _button;
    private P2Card _card;
    private Func<P2Card, bool> _canSelect;
    private Action<P2Card> _onClicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(HandleClick);
    }

    public void Setup(P2Card card, Func<P2Card, bool> canSelect, Action<P2Card> onClicked)
    {
        _card = card;
        _canSelect = canSelect;
        _onClicked = onClicked;

        if (nameText != null)
            nameText.text = card != null ? card.cardName : "(null)";

        Refresh();
    }

    public void Refresh()
    {
        if (icon != null)
        {
            icon.sprite = GetCardArtSprite(_card);
            icon.enabled = icon.sprite != null;
        }

        bool interactable = _card != null && (_canSelect == null || _canSelect(_card));

        if (_button != null)
            _button.interactable = interactable;

        if (canvasGroup != null)
            canvasGroup.alpha = interactable ? 1f : disabledAlpha;
    }

    private static Sprite GetCardArtSprite(P2Card c)
    {
        if (c == null)
            return null;

        if (c.effect != null && c.effect.CardArtOverride != null)
            return c.effect.CardArtOverride;

        return c.icon;
    }

    private void HandleClick()
    {
        if (_card == null)
            return;

        _onClicked?.Invoke(_card);
    }
}