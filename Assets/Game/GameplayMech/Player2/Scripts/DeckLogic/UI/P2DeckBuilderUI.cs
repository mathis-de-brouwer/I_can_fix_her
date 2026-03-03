using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class P2DeckBuilderUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private P2CardDatabase cardDatabase;
    [SerializeField] private int requiredSeedCount = 5;

    [Header("Config")]
    [SerializeField] private int startingDeckSize = 10;
    [SerializeField] private int startingHandSize = 5;

    [Header("UI - Available cards list")]
    [SerializeField] private Transform availableCardsContent;
    [SerializeField] private P2DeckBuilderCardTileUI availableCardTilePrefab;

    [Header("UI - Selected slots")]
    [SerializeField] private Transform selectedSlotsContent;
    [SerializeField] private P2DeckBuilderSelectedSlotUI selectedSlotPrefab;

    [Header("UI - Actions")]
    [SerializeField] private Button playButton;
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Tooltip")]
    [SerializeField] private TooltipController tooltipController;

    private readonly List<P2Card> _selected = new List<P2Card>();
    private readonly List<P2DeckBuilderCardTileUI> _tiles = new List<P2DeckBuilderCardTileUI>();
    private readonly List<P2DeckBuilderSelectedSlotUI> _slots = new List<P2DeckBuilderSelectedSlotUI>();

    private void Start()
    {
        MusicService.Play(MusicId.DeckBuilder);

        BuildAvailableList();
        BuildSlots();
        RefreshAll();
    }

    private void BuildAvailableList()
    {
        if (availableCardsContent == null || availableCardTilePrefab == null || cardDatabase == null)
            return;

        for (int i = availableCardsContent.childCount - 1; i >= 0; i--)
            Destroy(availableCardsContent.GetChild(i).gameObject);

        _tiles.Clear();

        List<P2Card> cards = cardDatabase.cards;
        if (cards == null)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            P2Card card = cards[i];
            if (card == null)
                continue;

            P2DeckBuilderCardTileUI tile = Instantiate(availableCardTilePrefab, availableCardsContent);
            tile.Setup(card, CanSelectCard, SelectCard, tooltipController);
            _tiles.Add(tile);
        }
    }

    private void BuildSlots()
    {
        if (selectedSlotsContent == null || selectedSlotPrefab == null)
            return;

        for (int i = selectedSlotsContent.childCount - 1; i >= 0; i--)
            Destroy(selectedSlotsContent.GetChild(i).gameObject);

        _slots.Clear();

        int count = Mathf.Max(1, requiredSeedCount);
        for (int i = 0; i < count; i++)
        {
            P2DeckBuilderSelectedSlotUI slot = Instantiate(selectedSlotPrefab, selectedSlotsContent);
            slot.Setup(i, RemoveAt);
            _slots.Add(slot);
        }
    }

    private bool CanSelectCard(P2Card card)
    {
        if (card == null)
            return false;

        return _selected.Count < requiredSeedCount && !_selected.Contains(card);
    }

    private void SelectCard(P2Card card)
    {
        if (!CanSelectCard(card))
            return;

        _selected.Add(card);
        RefreshAll();
    }

    private void RemoveAt(int index)
    {
        if (index < 0 || index >= _selected.Count)
            return;

        _selected.RemoveAt(index);
        RefreshAll();
    }

    private void RefreshAll()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            P2Card card = i < _selected.Count ? _selected[i] : null;
            _slots[i].SetCard(card);
        }

        for (int i = 0; i < _tiles.Count; i++)
            _tiles[i].Refresh();

        if (playButton != null)
            playButton.interactable = _selected.Count == requiredSeedCount;
    }

    public void PlaySeededDeck()
    {
        UiSfx.PlayClick();

        if (_selected.Count != requiredSeedCount)
            return;

        EnsureSessionExists();
        P2DeckSelectionSession.Instance.SetConfig(new P2DeckBuildConfig
        {
            mode = P2DeckBuildMode.SeedListWithRandomDuplicates,
            startingDeckSize = Mathf.Max(1, startingDeckSize),
            startingHandSize = Mathf.Max(1, startingHandSize),
            seedCards = new List<P2Card>(_selected),
        });

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void PlayRandomDeck()
    {
        UiSfx.PlayClick();

        EnsureSessionExists();
        P2DeckSelectionSession.Instance.SetConfig(new P2DeckBuildConfig
        {
            mode = P2DeckBuildMode.RandomFromAllAvailable,
            startingDeckSize = Mathf.Max(1, startingDeckSize),
            startingHandSize = Mathf.Max(1, startingHandSize),
        });

        SceneManager.LoadScene(gameplaySceneName);
    }

    private static void EnsureSessionExists()
    {
        if (P2DeckSelectionSession.Instance != null)
            return;

        GameObject go = new GameObject(nameof(P2DeckSelectionSession));
        go.AddComponent<P2DeckSelectionSession>();
    }
}