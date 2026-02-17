using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2DeckManager : MonoBehaviour
{
    [Header("All possible cards in the game")]
    public List<P2Card> allAvailableCards = new List<P2Card>();

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

    public bool IsBusy => _isResolvingPlay;

    private void Awake()
    {
        int actualSeed = seed != 0 ? seed : Environment.TickCount;
        _rng = new System.Random(actualSeed);
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
    }

    public void GenerateStartingDeck()
    {
        deck.Clear();
        hand.Clear();
        usedPile.Clear();

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

        if (config.seedCards == null || config.seedCards.Count == 0)
        {
            Debug.LogWarning("Seed deck is empty. Falling back to random generation.");
            GenerateStartingDeck();
            return;
        }

        List<P2Card> validSeeds = new List<P2Card>();
        for (int i = 0; i < config.seedCards.Count; i++)
        {
            if (config.seedCards[i] != null)
                validSeeds.Add(config.seedCards[i]);
        }

        if (validSeeds.Count == 0)
        {
            Debug.LogWarning("Seed deck contains only nulls. Falling back to random generation.");
            GenerateStartingDeck();
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

        P2Card drawnCard = deck[0];
        deck.RemoveAt(0);
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

        float cost = Mathf.Max(0f, card.cost);
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

        if (!CanAfford(card))
        {
            Debug.LogWarning($"Cannot play {card.cardName} - not enough charges (cost {card.cost:F2}).");
            return false;
        }

        StartCoroutine(ResolvePlayRoutine(card));
        return true;
    }

    private IEnumerator ResolvePlayRoutine(P2Card card)
    {
        _isResolvingPlay = true;

        if (charges != null)
            charges.Spend(Mathf.Max(0f, card.cost));

        hand.Remove(card);
        usedPile.Add(card);

        if (effectResolver != null)
            effectResolver.Resolve(card);

        RaiseDeckStateChanged();

        if (playResolveDelay > 0f)
            yield return new WaitForSeconds(playResolveDelay);

        DrawCard();

        Debug.Log($"Played card: {card.cardName} (cost = {card.cost:F2}).");

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
    }
}
