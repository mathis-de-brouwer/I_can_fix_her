using UnityEngine;

[System.Serializable]
public sealed class P2CardEffectContext
{
    public Transform spawnPoint;
    public P2Charges charges;
    public MonoBehaviour coroutineRunner;

    public Transform target;
}