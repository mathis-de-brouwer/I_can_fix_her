using TMPro;
using UnityEngine;

public sealed class P2DeckVisualUI : MonoBehaviour
{
    [SerializeField] private P2DeckManager deckManager;

    [Header("Counts")]
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private TMP_Text discardCountText;

    [Header("Optional pile visuals (UI Images)")]
    [SerializeField] private RectTransform deckPile;
    [SerializeField] private RectTransform discardPile;

    [SerializeField] private int expectedMaxDeckSize = 20;
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;

    private void OnEnable()
    {
        if (deckManager != null)
            deckManager.DeckStateChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (deckManager != null)
            deckManager.DeckStateChanged -= Refresh;
    }

    private void Refresh()
    {
        if (deckManager == null)
            return;

        if (deckCountText != null)
            deckCountText.text = deckManager.deck != null ? deckManager.deck.Count.ToString() : "0";

        if (discardCountText != null)
            discardCountText.text = deckManager.usedPile != null ? deckManager.usedPile.Count.ToString() : "0";

        UpdateScale(deckPile, deckManager.deck != null ? deckManager.deck.Count : 0);
        UpdateScale(discardPile, deckManager.usedPile != null ? deckManager.usedPile.Count : 0);
    }

    private void UpdateScale(RectTransform t, int count)
    {
        if (t == null || expectedMaxDeckSize <= 0)
            return;

        float lerp = Mathf.Clamp01(count / (float)expectedMaxDeckSize);
        float scale = Mathf.Lerp(minScale, maxScale, lerp);
        t.localScale = new Vector3(scale, scale, 1f);
    }
}