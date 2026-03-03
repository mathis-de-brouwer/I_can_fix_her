using System;
using System.Collections.Generic;
using UnityEngine;

public class P1ItemChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<P1ItemOptionUI> optionSlots;

    P1ItemChoiceNavigator _navigator;

    Action<P1RewardOffer> _onChose;
    bool _confirmed;

    public IReadOnlyList<P1ItemOptionUI> OptionSlots => optionSlots;

    void Awake()
    {
        _navigator = GetComponent<P1ItemChoiceNavigator>();
    }

    public void Show(List<P1RewardOffer> offers, Action<P1RewardOffer> onChose)
    {
        _onChose = onChose;
        _confirmed = false;

        panel.SetActive(true);

        for (int i = 0; i < optionSlots.Count; i++)
        {
            if (i < offers.Count)
            {
                optionSlots[i].Setup(offers[i], OnOptionClicked);
                optionSlots[i].SetMouseInputEnabled(false); // controller-only for now
            }
            else
            {
                optionSlots[i].gameObject.SetActive(false);
            }
        }

        _navigator?.Activate();
    }

    public void Hide()
    {
        _navigator?.Deactivate();
        panel.SetActive(false);
    }

    void OnOptionClicked(P1RewardOffer offer)
    {
        if (_confirmed)
            return;

        _confirmed = true;

        UiSfx.PlayRewardChoice();

        for (int i = 0; i < optionSlots.Count; i++)
            optionSlots[i].SetDimmed(optionSlots[i].Offer != offer);

        _navigator?.Deactivate();
        _onChose?.Invoke(offer);
    }
}