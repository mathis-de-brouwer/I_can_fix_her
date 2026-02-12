using System;
using System.Collections.Generic;
using UnityEngine;

public class P2DeckManager : MonoBehaviour
{
    [Header("All possible cards in the game")]
    public List<P2Card> allAvailableCards = new List<P2Card>();

    [Header("Match settings")]
    public int startingDeckSize = 10;
    public int startingHandSize = 5;

    [Header("Random")]
    [Tooltip("0 = auto seed. Any other value makes deck/shuffle deterministic.")]
    [SerializeField] private int seed;

    [Header("Runtime state")]
    public List<P2Card> deck = new List<P2Card>();
    public List<P2Card> hand = new List<P2Card>();
    public List<P2Card> usedPile = new List<P2Card>();

    public event Action HandChanged;

    private System.Random _rng;

    [Header("Card effects")]
    [SerializeField] private P2CardEffectResolver effectResolver;

    [Header("Charges")]
    [SerializeField] private P2Charges charges;

    private void Awake()
    {
        int actualSeed = seed != 0 ? seed : Environment.TickCount;
        _rng = new System.Random(actualSeed);
    }

    private void Start()
    {
        GenerateStartingDeck();
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
            HandChanged?.Invoke();
            return;
        }

        for (int i = 0; i < startingDeckSize; i++)
        {
            int index = _rng.Next(0, allAvailableCards.Count);
            deck.Add(allAvailableCards[index]);
        }

        Debug.Log($"Generated deck with {deck.Count} cards.");

        HandChanged?.Invoke();
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
    }

    public void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }

        Debug.Log($"Hand drawn with {hand.Count} cards.");

        HandChanged?.Invoke();
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

        if (charges != null)
            charges.Spend(Mathf.Max(0f, card.cost));

        hand.Remove(card);
        usedPile.Add(card);

        if (effectResolver != null)
            effectResolver.Resolve(card);

        DrawCard();

        Debug.Log($"Played card: {card.cardName} (cost = {card.cost:F2}).");

        HandChanged?.Invoke();
        return true;
    }

    public bool IsDeckEmpty()
    {
        return deck.Count == 0;
    }
}
