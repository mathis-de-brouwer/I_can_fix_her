using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "P2/Card Database")]
public sealed class P2CardDatabase : ScriptableObject
{
    [Tooltip("Every card that exists in the game. Assign once here; both the deck manager and deck builder read from this.")]
    public List<P2Card> cards = new List<P2Card>();
}