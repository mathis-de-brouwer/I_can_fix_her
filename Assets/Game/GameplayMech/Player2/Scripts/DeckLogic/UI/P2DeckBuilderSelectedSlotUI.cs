using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class P2DeckBuilderSelectedSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    private int _index;
    private Action<int> _onClicked;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(HandleClick);
    }

    public void Setup(int index, Action<int> onClicked)
    {
        _index = index;
        _onClicked = onClicked;
    }

    public void SetCard(P2Card card)
    {
        if (icon == null)
            return;

        icon.enabled = card != null;
        icon.sprite = GetCardArtSprite(card);
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
        UiSfx.PlayDeckbuilderCard();
        _onClicked?.Invoke(_index);
    }
}