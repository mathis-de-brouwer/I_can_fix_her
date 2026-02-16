using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P2HandUI : MonoBehaviour
{
    [SerializeField] private P2DeckManager deckManager;
    [SerializeField] private GameObject cardButtonPrefab;
    [SerializeField] private RectTransform handContent; // assign the HandContent container (optional)
    [SerializeField] private TMP_Text deckCountText;     // optional deck counter

    private void OnEnable()
    {
        if (deckManager != null)
            deckManager.HandChanged += RefreshHand;

        RefreshHand();
    }

    private void OnDisable()
    {
        if (deckManager != null)
            deckManager.HandChanged -= RefreshHand;
    }

    public void RefreshHand()
    {
        if (deckManager == null || cardButtonPrefab == null)
            return;

        Transform container = handContent != null ? handContent : transform;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        foreach (P2Card card in deckManager.hand)
        {
            GameObject newCard = Instantiate(cardButtonPrefab, container);
            newCard.transform.localScale = Vector3.one;
            P2CardUI ui = newCard.GetComponent<P2CardUI>();
            if (ui != null)
                ui.Setup(card, deckManager);
        }

        if (deckCountText != null && deckManager.deck != null)
            deckCountText.text = deckManager.deck.Count.ToString();
    }
}
