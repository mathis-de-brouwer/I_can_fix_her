using TMPro;
using UnityEngine;

public sealed class P2DeckVisualUI : MonoBehaviour
{
    [SerializeField] private P2DeckManager deckManager;

    [Header("Counts")]
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private TMP_Text discardCountText;

    [Header("Optional pile visuals (UI Transforms)")]
    [SerializeField] private RectTransform deckPile;
    [SerializeField] private RectTransform discardPile;

    [Header("Pile back sprite")]
    [SerializeField] private Sprite cardBackSprite;

    [Header("Pile stacks (optional)")]
    [SerializeField] private P2PileStackUI deckStack;
    [SerializeField] private P2PileStackUI discardStack;

    [SerializeField] private int expectedMaxDeckSize = 20;
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;

    private void OnEnable()
    {
        ApplyBackSprites();

        if (deckManager != null)
            deckManager.DeckStateChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (deckManager != null)
            deckManager.DeckStateChanged -= Refresh;
    }

    private void ApplyBackSprites()
    {
        if (cardBackSprite == null)
            return;

        if (deckStack != null)
            deckStack.SetSprite(cardBackSprite);

        if (discardStack != null)
            discardStack.SetSprite(cardBackSprite);
    }

    private void Refresh()
    {
        if (deckManager == null)
            return;

        int deckCount = deckManager.deck != null ? deckManager.deck.Count : 0;
        int discardCount = deckManager.usedPile != null ? deckManager.usedPile.Count : 0;

        if (deckCountText != null)
            deckCountText.text = deckCount.ToString();

        if (discardCountText != null)
            discardCountText.text = discardCount.ToString();

        if (deckStack != null)
            deckStack.SetCount(deckCount);

        if (discardStack != null)
            discardStack.SetCount(discardCount);

        UpdateScale(deckPile, deckCount);
        UpdateScale(discardPile, discardCount);
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