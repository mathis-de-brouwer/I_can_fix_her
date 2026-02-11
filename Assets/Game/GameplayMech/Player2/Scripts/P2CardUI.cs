using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P2CardUI : MonoBehaviour
{
    public P2Card card;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text costText;
    public TMP_Text durationText;

    private P2DeckManager deckManager;

    public void Setup(P2Card newCard, P2DeckManager manager)
    {
        card = newCard;
        deckManager = manager;

        icon.sprite = card.icon;
        nameText.text = card.cardName;
        costText.text = card.cost.ToString();
        durationText.text = card.duration.ToString();
    }

    public void OnClick()
    {
        if (card == null || deckManager == null)
            return;

        if (card.prefab != null)
        {
            Vector3 spawnPos = Vector3.zero; // temp
            Instantiate(card.prefab, spawnPos, Quaternion.identity);
        }

        deckManager.PlayCard(card);
    }
}
