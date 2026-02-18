using System.Collections.Generic;
using UnityEngine;

/// <summary>Static helpers for working with a list of <see cref="P2SpawnEntry"/>.</summary>
public static class SpawnTable
{
    /// <summary>Returns a random prefab from <paramref name="entries"/> using weighted probability.</summary>
    public static GameObject Pick(List<P2SpawnEntry> entries)
    {
        if (entries == null || entries.Count == 0)
            return null;

        float total = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i] != null && entries[i].prefab != null)
                total += entries[i].weight;
        }

        if (total <= 0f)
            return null;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < entries.Count; i++)
        {
            P2SpawnEntry entry = entries[i];
            if (entry == null || entry.prefab == null)
                continue;

            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.prefab;
        }

        // Fallback: return the last valid prefab
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            if (entries[i] != null && entries[i].prefab != null)
                return entries[i].prefab;
        }

        return null;
    }

    /// <summary>Returns true if the list has at least one valid entry.</summary>
    public static bool IsValid(List<P2SpawnEntry> entries)
    {
        if (entries == null || entries.Count == 0)
            return false;

        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i] != null && entries[i].prefab != null)
                return true;
        }

        return false;
    }
}