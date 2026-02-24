    using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// P1's level-up panel. Clicking an item immediately confirms the choice.
/// </summary>
public class P1ItemChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<P1ItemOptionUI> optionSlots;

    Action<GameObject> _onChose;

    public void Show(List<GameObject> offers, Action<GameObject> onChose)
    {
        _onChose = onChose;
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

    void OnOptionClicked(GameObject prefab)
    {
        // Dim all others for visual feedback, then immediately confirm
        for (int i = 0; i < optionSlots.Count; i++)
            optionSlots[i].SetDimmed(optionSlots[i].Prefab != prefab);

        _onChose?.Invoke(prefab);
    }
}