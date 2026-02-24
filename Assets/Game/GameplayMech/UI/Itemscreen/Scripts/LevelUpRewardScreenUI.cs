using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiated at runtime by <see cref="LevelUpRewardManager"/>.
/// Owns both choice panels and destroys itself once both players have chosen.
/// </summary>
public class LevelUpRewardScreenUI : MonoBehaviour
{
    [Header("P1 - left panel")]
    [SerializeField] private P1ItemChoiceUI p1ChoiceUI;

    [Header("P2 - right panel")]
    [SerializeField] private P2CardChoiceUI p2ChoiceUI;

    bool _p1Done;
    bool _p2Done;

    /// <summary>Called by <see cref="LevelUpRewardManager"/> right after Instantiate().</summary>
    public void Setup(List<GameObject> p1Offers, List<P2Card> p2Offers,
                      Action<GameObject> onP1Chose, Action<P2Card> onP2Chose)
    {
        _p1Done = false;
        _p2Done = false;

        if (p1ChoiceUI == null) Debug.LogError("LevelUpRewardScreenUI: p1ChoiceUI is not assigned!");
        if (p2ChoiceUI == null) Debug.LogError("LevelUpRewardScreenUI: p2ChoiceUI is not assigned!");

        p1ChoiceUI.Show(p1Offers, prefab =>
        {
            Debug.Log("P1 confirmed item choice.");
            onP1Chose?.Invoke(prefab);
            _p1Done = true;
            TryClose();
        });

        p2ChoiceUI.Show(p2Offers, card =>
        {
            Debug.Log("P2 confirmed card choice.");
            onP2Chose?.Invoke(card);
            _p2Done = true;
            TryClose();
        });
    }

    void TryClose()
    {
        Debug.Log($"TryClose — P1: {_p1Done}, P2: {_p2Done}");

        if (!_p1Done || !_p2Done)
            return;

        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}