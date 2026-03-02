using UnityEngine;

public sealed class EnemySpawnDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Playermovement targetPlayer;
    [SerializeField] private P2MatchClock matchClock;

    [Header("Config")]
    [SerializeField] private EnemySpawnDirectorConfig config;

    private float _spawnAccumulatorSeconds;

    private void Awake()
    {
        if (targetPlayer == null)
            targetPlayer = FindAnyObjectByType<Playermovement>();

        if (matchClock == null)
            matchClock = FindAnyObjectByType<P2MatchClock>();
    }

    private void Update()
    {
        if (config == null)
            return;

        if (targetPlayer == null)
            return;

        if (!SpawnTable.IsValid(config.spawnTable))
            return;

        float elapsedSeconds = matchClock != null ? matchClock.ElapsedSeconds : Time.timeSinceLevelLoad;

        float magnitudeMult = 1f;
        if (config.timeScaling != null)
            magnitudeMult = config.timeScaling.GetMagnitudeMultiplier(elapsedSeconds);

        float spawnsPerSecond = Mathf.Max(0f, config.baseSpawnsPerSecond * magnitudeMult * config.spawnsPerSecondScale);
        int maxAlive = Mathf.Max(0, Mathf.RoundToInt(config.baseMaxAlive * magnitudeMult * config.maxAliveScale));

        if (spawnsPerSecond <= 0f)
            return;

        _spawnAccumulatorSeconds += Time.deltaTime;

        float interval = 1f / spawnsPerSecond;

        int safety = 0;
        while (_spawnAccumulatorSeconds >= interval && safety < 50)
        {
            _spawnAccumulatorSeconds -= interval;
            safety++;

            if (maxAlive > 0 && EnemySpawnRegistry.AliveCount >= maxAlive)
                break;

            SpawnOne(targetPlayer.transform.position);
        }
    }

    private void SpawnOne(Vector3 playerPos)
    {
        GameObject prefab = SpawnTable.Pick(config.spawnTable);
        if (prefab == null)
            return;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float dist = config.spawnRadius + Random.Range(-config.randomJitter, config.randomJitter);

        Vector3 pos = playerPos + new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);

        GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
        EnemySpawnRegistry.Register(instance);
    }
}