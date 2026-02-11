using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P2HandUI : MonoBehaviour
{
    [SerializeField] private P2DeckManager deckManager;
    [SerializeField] private GameObject cardButtonPrefab;

    [Header("UI container (holds only spawned card buttons)")]
    [SerializeField] private RectTransform handContent;

    [Header("Optional deck counter (left)")]
    [SerializeField] private TMP_Text deckCountText;

    private HorizontalLayoutGroup _horizontalLayout;

    private void Awake()
    {
        if (handContent != null)
            _horizontalLayout = handContent.GetComponent<HorizontalLayoutGroup>();
    }

    private void OnEnable() 
    {
        if (deckManager != null)
            deckManager.HandChanged += RefreshHand;

        // Helpful fallback for legacy scenes (warn so you fix it)
        if (handContent == null)
        {
            handContent = GetComponent<RectTransform>();
            Debug.LogWarning($"{nameof(P2HandUI)}: handContent not assigned — defaulting to this GameObject's RectTransform. Assign a dedicated HandContent to avoid accidental deletes.");
            if (handContent != null)
                _horizontalLayout = handContent.GetComponent<HorizontalLayoutGroup>();
        }

        RefreshHand();
    }

    private void OnDisable()
    {
        if (deckManager != null)
            deckManager.HandChanged -= RefreshHand;
    }

    public void RefreshHand()
    {
        if (deckManager == null || cardButtonPrefab == null || handContent == null)
            return;

        // Clear only the dedicated container children (safe)
        for (int i = handContent.childCount - 1; i >= 0; i--)
            Destroy(handContent.GetChild(i).gameObject);

        // Instantiate a card UI for each card in hand
        foreach (P2Card card in deckManager.hand)
        {
            GameObject newCard = Instantiate(cardButtonPrefab, handContent);
            // Normalize transform so LayoutGroups behave predictably
            RectTransform rt = newCard.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.localRotation = Quaternion.identity;
            }
            else
            {
                newCard.transform.localScale = Vector3.one;
                newCard.transform.localRotation = Quaternion.identity;
            }

            // If a HorizontalLayoutGroup is used, ensure a LayoutElement exists with a sensible preferred width
            if (_horizontalLayout != null)
            {
                LayoutElement le = newCard.GetComponent<LayoutElement>() ?? newCard.AddComponent<LayoutElement>();

                // If prefab already defines a size, prefer that; otherwise set a default width
                float prefWidth = (rt != null && rt.sizeDelta.x > 0f) ? rt.sizeDelta.x : 160f;
                if (le.preferredWidth <= 0f)
                    le.preferredWidth = prefWidth;

                if (le.preferredHeight <= 0f && rt != null && rt.sizeDelta.y > 0f)
                    le.preferredHeight = rt.sizeDelta.y;
            }

            P2CardUI cardUI = newCard.GetComponent<P2CardUI>();
            if (cardUI != null)
                cardUI.Setup(card, deckManager);
        }

        // Force immediate layout rebuild to avoid visual clumping until Unity performs its layout pass
        if (_horizontalLayout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(handContent);
            Canvas.ForceUpdateCanvases();
        }

        // Update deck count UI if assigned
        if (deckCountText != null && deckManager.deck != null)
            deckCountText.text = deckManager.deck.Count.ToString();
    }
}
