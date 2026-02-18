using UnityEngine;

/// <summary>
/// One entry in a spawn table. Assign a prefab and a relative weight
/// (higher weight = spawned more often). Used by any effect that can
/// spawn multiple different monster types.
/// </summary>
[System.Serializable]
public sealed class P2SpawnEntry
{
    [Tooltip("Enemy or object prefab to spawn.")]
    public GameObject prefab;

    [Tooltip("Relative spawn weight. e.g. weight 3 is 3x more likely than weight 1.")]
    [Min(0.01f)]
    public float weight = 1f;
}