using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Spawn Director Config")]
public sealed class EnemySpawnDirectorConfig : ScriptableObject
{
    [Header("Spawn table")]
    [Tooltip("Weighted enemies to pick from.")]
    public List<P2SpawnEntry> spawnTable = new List<P2SpawnEntry>();

    [Header("Spawn pacing (base values before scaling)")]
    [Min(0f)]
    public float baseSpawnsPerSecond = 0.5f;

    [Min(0)]
    public int baseMaxAlive = 120;

    [Header("Spawn placement (ring around player)")]
    [Min(0f)]
    public float spawnRadius = 9f;

    [Min(0f)]
    public float randomJitter = 0.75f;

    [Header("Scaling over time (curve-driven)")]
    [Tooltip("Uses GetMagnitudeMultiplier(elapsedSeconds). If null -> no scaling.")]
    public P2TimeScalingConfig timeScaling;

    [Tooltip("Extra multiplier applied to scaled spawn rate.")]
    [Min(0f)]
    public float spawnsPerSecondScale = 1f;

    [Tooltip("Extra multiplier applied to scaled max alive.")]
    [Min(0f)]
    public float maxAliveScale = 1f;
}