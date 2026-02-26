using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// P2's level-up panel. Clicking a card immediately confirms the choice.
/// </summary>
public class P2CardChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<P2CardOptionUI> optionSlots;

    Action<P2Card> _onChose;
    bool _confirmed;

    public void Show(List<P2Card> offers, Action<P2Card> onChose)
    {
        _onChose = onChose;
        _confirmed = false;

        panel.SetActive(true);

        for (int i = 0; i < optionSlots.Count; i++)
        {
            if (i < offers.Count)
                optionSlots[i].Setup(offers[i], OnOptionClicked);
            else
                optionSlots[i].gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    void OnOptionClicked(P2Card card)
    {
        if (_confirmed)
            return;

        _confirmed = true;

        // Dim all others for visual feedback, then immediately confirm
        for (int i = 0; i < optionSlots.Count; i++)
            optionSlots[i].SetDimmed(optionSlots[i].Card != card);

        _onChose?.Invoke(card);
    }
}