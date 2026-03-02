using UnityEngine;

public static class EnemySpawnRegistry
{
    private static int _aliveCount;

    public static int AliveCount => _aliveCount;

    public static void Register(GameObject enemyInstance)
    {
        if (enemyInstance == null)
            return;

        EnemySpawnRegistryHook hook = enemyInstance.GetComponent<EnemySpawnRegistryHook>();
        if (hook != null)
            return;

        enemyInstance.AddComponent<EnemySpawnRegistryHook>();
        _aliveCount++;
    }

    private sealed class EnemySpawnRegistryHook : MonoBehaviour
    {
        private void OnDestroy()
        {
            _aliveCount = Mathf.Max(0, _aliveCount - 1);
        }
    }
}