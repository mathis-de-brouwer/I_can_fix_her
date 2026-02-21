using UnityEngine;

[CreateAssetMenu(fileName = "ChunkGroup", menuName = "Map/Chunk Group Definition")]
public class ChunkGroupDefinition : ScriptableObject
{
    [System.Serializable]
    public struct ChunkEntry
    {
        [Tooltip("Offset in chunk-grid units from the anchor coord. (0,0) = the anchor itself.")]
        public Vector2Int gridOffset;
        public PropRandom prefab;
    }

    [Header("Layout")]
    [Tooltip("All chunks that spawn together as one structure.")]
    public ChunkEntry[] entries;

    [Header("Spawn Condition")]
    [Tooltip("Optional flag that must be set via MapController.SetFlag() before this group can spawn.")]
    public string requiredFlag = "";

    [Tooltip("How many times this group may appear on the map. -1 = unlimited.")]
    public int maxInstances = -1;
}