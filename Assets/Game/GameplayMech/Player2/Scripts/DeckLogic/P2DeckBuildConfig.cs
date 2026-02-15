using System.Collections.Generic;
using UnityEngine;

public enum P2DeckBuildMode
{
    RandomFromAllAvailable = 0,
    SeedListWithRandomDuplicates = 1,
}

[System.Serializable]
public sealed class P2DeckBuildConfig
{
    public P2DeckBuildMode mode = P2DeckBuildMode.RandomFromAllAvailable;

    [Min(1)]
    public int startingDeckSize = 10;

    [Min(1)]
    public int startingHandSize = 5;

    public List<P2Card> seedCards = new List<P2Card>();
}