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
        if (icon != null)
        {
            icon.enabled = card != null;
            icon.sprite = card != null ? card.icon : null;
        }
    }

    private void HandleClick()
    {
        _onClicked?.Invoke(_index);
    }
}