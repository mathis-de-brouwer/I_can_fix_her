using UnityEngine;

public sealed class MatchStats : MonoBehaviour
{
    public static MatchStats Instance { get; private set; }

    public int P1MonstersKilled { get; private set; }
    public float P2DamageDoneToP1 { get; private set; }
    public float ElapsedSeconds { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        ElapsedSeconds += Time.deltaTime;
    }

    public void ResetAll()
    {
        P1MonstersKilled = 0;
        P2DamageDoneToP1 = 0f;
        ElapsedSeconds = 0f;
    }

    public void RegisterMonsterKilled()
    {
        P1MonstersKilled++;
    }

    public void RegisterDamageToP1(float amount)
    {
        if (amount <= 0f)
            return;

        P2DamageDoneToP1 += amount;
    }
}