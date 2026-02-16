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

    [Header("Availability visuals")]
    [SerializeField] private float disabledAlpha = 0.45f;

    private P2DeckManager deckManager;
    private Button _button;
    private CanvasGroup _canvasGroup;

    public void Setup(P2Card newCard, P2DeckManager manager)
    {
        card = newCard;
        deckManager = manager;

        icon.sprite = card.icon;
        nameText.text = card.cardName;
        costText.text = card.cost.ToString();
        durationText.text = card.duration.ToString();

        CacheComponents();
        UpdateInteractivity();
    }

    private void Awake()
    {
        CacheComponents();
    }

    private void Update()
    {
        UpdateInteractivity();
    }

    private void CacheComponents()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void UpdateInteractivity()
    {
        if (deckManager == null || card == null)
            return;

        bool canPlay = deckManager.CanAfford(card);

        if (_button != null)
            _button.interactable = canPlay;

        if (_canvasGroup != null)
            _canvasGroup.alpha = canPlay ? 1f : disabledAlpha;
        else
            SetFallbackTint(canPlay);
    }

    private void SetFallbackTint(bool canPlay)
    {
        Color c = canPlay ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);

        if (icon != null)
            icon.color = c;

        if (nameText != null)
            nameText.color = c;

        if (costText != null)
            costText.color = c;

        if (durationText != null)
            durationText.color = c;
    }

    public void OnClick()
    {
        if (card == null || deckManager == null)
            return;

        deckManager.TryPlayCard(card);
    }
}
