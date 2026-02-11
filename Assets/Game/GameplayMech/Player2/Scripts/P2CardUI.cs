using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class P2CardUI : MonoBehaviour
{
    public P2Card card;
    public Image icon;
    public TMP_Text costText;

    public void Setup(P2Card newCard)
    {
        card = newCard;
        icon.sprite = card.icon;
        costText.text = card.cost.ToString();
    }

    void Awake()
    {
        if (card != null)
            Setup(card);
    }

    public void OnClick()
    {
        //Player2Controller.Instance.UseCard(card);
        if (card.prefab != null)
        {
            Vector3 spawnPos = new Vector3(0, 0, 0);
            Instantiate(card.prefab, spawnPos, Quaternion.identity);
        }
    }
}
