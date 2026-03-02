using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2DeckManager : MonoBehaviour
{
    [Header("Card Database")]
    [SerializeField] private P2CardDatabase cardDatabase;

    [Header("Match settings (used when no session config is provided)")]
    public int startingDeckSize = 10;
    public int startingHandSize = 5;

    [Header("Random")]
    [Tooltip("0 = auto seed. Any other value makes deck/shuffle deterministic.")]
    [SerializeField] private int seed;

    [Header("Runtime state")]
    public List<P2Card> deck = new List<P2Card>();
    public List<P2Card> hand = new List<P2Card>();
    public List<P2Card> usedPile = new List<P2Card>();

    [Header("Play flow")]
    [SerializeField] private float playResolveDelay = 0.2f;

    public event Action HandChanged;
    public event Action DeckStateChanged;

    private System.Random _rng;
    private bool _isResolvingPlay;

    [Header("Card effects")]
    [SerializeField] private P2CardEffectResolver effectResolver;

    [Header("Charges")]
    [SerializeField] private P2Charges charges;

    [Header("Time scaling")]
    [SerializeField] private P2MatchClock matchClock;
    [SerializeField] private P2TimeScalingConfig timeScaling;
    [SerializeField] private float minScaledCost = 0.1f;

    [Header("Game Result")]
    [SerializeField] private GameObject gameResultPrefab; // same prefab as PlayerStats

    [Header("Final boss (when P2 runs out of cards)")]
    [Tooltip("Granted once when both deck and hand are empty. Recommended: cost = 0, scaleCostWithTime = false.")]
    [SerializeField] private P2Card bossCard;

    private bool _gameStarted = false;
    private bool _bossCardGranted;
    private bool _bossAlive;

    public bool IsBusy => _isResolvingPlay;

    public List<P2Card> allAvailableCards => cardDatabase != null ? cardDatabase.cards : new List<P2Card>();

    private void Awake()
    {
        int actualSeed = seed != 0 ? seed : Environment.TickCount;
        _rng = new System.Random(actualSeed);

        if (matchClock == null)
            matchClock = FindAnyObjectByType<P2MatchClock>();

        if (timeScaling == null && effectResolver != null)
            timeScaling = effectResolver.TimeScaling;
    }

    public float GetScaledCost(P2Card card)
    {
        if (card == null)
            return 0f;

        float baseCost = Mathf.Max(0f, card.cost);

        if (!card.scaleCostWithTime)
            return baseCost;

        if (timeScaling == null)
            return baseCost;

        float t = matchClock != null ? matchClock.ElapsedSeconds : 0f;
        float multiplier = timeScaling.GetCostMultiplier(t);

        float scaled = baseCost * multiplier;
        if (scaled > 0f)
            scaled = Mathf.Max(minScaledCost, scaled);

        return scaled;
    }

    private void Start()
    {
        P2DeckBuildConfig config = P2DeckSelectionSession.Instance != null ? P2DeckSelectionSession.Instance.Config : null;

        if (config != null)
        {
            startingDeckSize = Mathf.Max(1, config.startingDeckSize);
            startingHandSize = Mathf.Max(1, config.startingHandSize);

            GenerateStartingDeck(config);
        }
        else
        {
            GenerateStartingDeck();
        }

        ShuffleDeck();
        DrawStartingHand();
        StartCoroutine(EnableGameStartedNextFrame());
    }

    private IEnumerator EnableGameStartedNextFrame()
    {
        yield return null;
        _gameStarted = true;
    }

    public void GenerateStartingDeck()
    {
        deck.Clear();
        hand.Clear();
        usedPile.Clear();

        _bossCardGranted = false;
        _bossAlive = false;

        if (allAvailableCards == null || allAvailableCards.Count == 0)
        {
            Debug.LogError("No available cards assigned. Cannot generate deck.");
            RaiseDeckStateChanged();
            return;
        }

        for (int i = 0; i < startingDeckSize; i++)
        {
            int index = _rng.Next(0, allAvailableCards.Count);
            deck.Add(allAvailableCards[index]);
        }

        Debug.Log($"Generated deck with {deck.Count} cards.");
        RaiseDeckStateChanged();
    }

    public void GenerateStartingDeck(P2DeckBuildConfig config)
    {
        deck.Clear();
        hand.Clear();
        usedPile.Clear();

        _bossCardGranted = false;
        _bossAlive = false;

        if (config == null)
        {
            GenerateStartingDeck();
            return;
        }

        switch (config.mode)
        {
            case P2DeckBuildMode.SeedListWithRandomDuplicates:
                GenerateSeededDeck(config);
                break;

            case P2DeckBuildMode.RandomFromAllAvailable:
            default:
                GenerateStartingDeck();
                break;
        }

        RaiseDeckStateChanged();
    }

    private void GenerateSeededDeck(P2DeckBuildConfig config)
    {
        int targetSize = Mathf.Max(1, config.startingDeckSize);

        List<P2Card> validSeeds = new List<P2Card>();

        if (config.seedCards != null)
        {
            for (int i = 0; i < config.seedCards.Count; i++)
            {
                if (config.seedCards[i] != null)
                    validSeeds.Add(config.seedCards[i]);
            }
        }

        if (validSeeds.Count == 0)
        {
            Debug.LogError("Seeded deck has no valid cards. Deck will be empty.");
            RaiseDeckStateChanged();
            return;
        }

        for (int i = 0; i < targetSize; i++)
        {
            int index = _rng.Next(0, validSeeds.Count);
            deck.Add(validSeeds[index]);
        }

        Debug.Log($"Generated seeded deck with {deck.Count} cards from {validSeeds.Count} seed cards.");
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = _rng.Next(i, deck.Count);
            P2Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        Debug.Log("Deck shuffled.");
        RaiseDeckStateChanged();
    }

    public void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }

        Debug.Log($"Hand drawn with {hand.Count} cards.");
        RaiseDeckStateChanged();
    }

    public P2Card DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty, cannot draw.");
            return null;
        }

        int last = deck.Count - 1;
        P2Card drawnCard = deck[last];
        deck.RemoveAt(last);
        hand.Add(drawnCard);

        Debug.Log($"Drew card: {drawnCard.cardName}");
        RaiseDeckStateChanged();

        return drawnCard;
    }

    public bool CanAfford(P2Card card)
    {
        if (card == null)
            return false;

        if (charges == null)
            return true;

        float cost = GetScaledCost(card);
        return charges.CanSpend(cost);
    }

    public bool TryPlayCard(P2Card card)
    {
        if (_isResolvingPlay)
            return false;

        if (card == null)
        {
            Debug.LogWarning("TryPlayCard called with null card.");
            return false;
        }

        if (!hand.Contains(card))
        {
            Debug.LogWarning("Tried to play a card that is not in hand.");
            return false;
        }

        float cost = GetScaledCost(card);

        if (!CanAfford(card))
        {
            Debug.LogWarning($"Cannot play {card.cardName} - not enough charges (cost {cost:F2}).");
            return false;
        }

        StartCoroutine(ResolvePlayRoutine(card));
        return true;
    }

    private IEnumerator ResolvePlayRoutine(P2Card card)
    {
        _isResolvingPlay = true;

        float cost = GetScaledCost(card);

        if (charges != null)
            charges.Spend(cost);

        hand.Remove(card);
        usedPile.Add(card);

        if (effectResolver != null)
            effectResolver.Resolve(card);

        RaiseDeckStateChanged();

        if (playResolveDelay > 0f)
            yield return new WaitForSeconds(playResolveDelay);

        DrawCard();

        Debug.Log($"Played card: {card.cardName} (cost = {cost:F2}).");

        RaiseDeckStateChanged();

        _isResolvingPlay = false;
    }

    public bool IsDeckEmpty()
    {
        return deck.Count == 0;
    }

    private void RaiseDeckStateChanged()
    {
        HandChanged?.Invoke();
        DeckStateChanged?.Invoke();
        CheckDeckEmpty();
    }

    private void CheckDeckEmpty()
    {
        if (!_gameStarted)
            return;

        if (deck.Count != 0 || hand.Count != 0)
            return;

        if (!_bossCardGranted)
        {
            _bossCardGranted = true;

            if (bossCard == null)
            {
                Debug.LogWarning($"{nameof(P2DeckManager)}: deck+hand empty but bossCard is not assigned. P2 loses normally.");
                EndMatchP2OutOfCards();
                return;
            }

            Debug.Log($"{nameof(P2DeckManager)}: deck+hand empty -> granting final boss card '{bossCard.cardName}'.");

            hand.Add(bossCard);

            // Refresh UI without re-entering CheckDeckEmpty via RaiseDeckStateChanged().
            HandChanged?.Invoke();
            DeckStateChanged?.Invoke();

            return;
        }

        // Boss card already granted. If the boss hasn't spawned yet, allow P2 to play it (hand will contain it).
        // If the boss is alive, do not end match here.
        if (_bossAlive)
            return;

        // Boss was granted and is no longer alive => boss defeated => P2 loses.
        EndMatchP2OutOfCards();
    }

    private void EndMatchP2OutOfCards()
    {
        if (gameResultPrefab == null)
            return;

        GameObject instance = Instantiate(gameResultPrefab);
        GameResultScreenUI ui = instance.GetComponent<GameResultScreenUI>();
        if (ui != null)
            ui.Setup(GameResultScreenUI.Winner.P1);
    }

    public void NotifyBossSpawned()
    {
        _bossAlive = true;
    }

    public void NotifyBossDied()
    {
        _bossAlive = false;

        // If P2 has no cards left at this moment, this will end the match.
        CheckDeckEmpty();
    }

    /// <summary>Adds a card directly to the deck (used by the level-up reward system).</summary>
    public void AddCardToDeck(P2Card card)
    {
        if (card == null) return;
        deck.Add(card);
        Debug.Log($"Added card to deck: {card.cardName} (deck size: {deck.Count})");
        RaiseDeckStateChanged();
    }
}
