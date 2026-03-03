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

    [Header("Icon layout")]
    [SerializeField] private bool forceIconStretch = true;
    [SerializeField] private bool preserveIconAspect;

    private P2DeckManager deckManager;
    private Button _button;
    private CanvasGroup _canvasGroup;
    private HoverTooltipSource _tooltipSource;

    public void Setup(P2Card newCard, P2DeckManager manager)
    {
        card = newCard;
        deckManager = manager;

        if (icon != null)
        {
            icon.sprite = GetCardArtSprite(card);
            ApplyIconLayout();
        }

        if (nameText != null)
            nameText.text = card != null ? card.cardName : string.Empty;

        if (costText != null)
            costText.text = card != null ? card.cost.ToString() : string.Empty;

        if (durationText != null)
            durationText.text = card != null ? card.duration.ToString() : string.Empty;

        CacheComponents();

        if (_tooltipSource != null && card != null)
            _tooltipSource.SetContent(GetCardArtSprite(card), card.cardName, card.description);

        UpdateInteractivity();
    }

    private void ApplyIconLayout()
    {
        if (icon == null)
            return;

        icon.preserveAspect = preserveIconAspect;

        if (!forceIconStretch)
            return;

        RectTransform rt = icon.rectTransform;
        if (rt == null)
            return;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;
    }

    private static Sprite GetCardArtSprite(P2Card c)
    {
        if (c == null)
            return null;

        if (c.effect != null && c.effect.CardArtOverride != null)
            return c.effect.CardArtOverride;

        return c.icon;
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

        if (_tooltipSource == null)
            _tooltipSource = GetComponent<HoverTooltipSource>();
    }

    private void UpdateInteractivity()
    {
        if (deckManager == null || card == null)
            return;

        bool canPlay = !deckManager.IsBusy && deckManager.CanAfford(card);

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

        UiSfx.PlayGameplayCard();
        deckManager.TryPlayCard(card);
    }
}
