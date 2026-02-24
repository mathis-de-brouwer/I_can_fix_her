using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for P1 level-ups and instantiates the reward screen prefab every
/// <see cref="levelsPerReward"/> levels. Mirrors the GameResultScreenUI pattern.
/// </summary>
public class LevelUpRewardManager : MonoBehaviour
{
    public static LevelUpRewardManager Instance { get; private set; }

    [Header("Trigger")]
    [Tooltip("Reward fires every N levels (e.g. 3 = levels 3, 6, 9 ...).")]
    public int levelsPerReward = 3;

    [Header("Databases")]
    [SerializeField] private PassiveItemDatabase passiveItemDatabase;
    [SerializeField] private P2CardDatabase cardDatabase;

    [Header("Prefab")]
    [Tooltip("Drag the LevelUpRewardScreen prefab here — instantiated at runtime like the death screen.")]
    [SerializeField] private GameObject levelUpRewardScreenPrefab;

    bool _rewardActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Called by <see cref="PlayerStats"/> after every level-up.</summary>
    public void NotifyLevelUp(int newLevel)
    {
        if (_rewardActive)
            return;

        if (newLevel % levelsPerReward != 0)
            return;

        ShowRewardScreen();
    }

    void ShowRewardScreen()
    {
        _rewardActive = true;
        Time.timeScale = 0f;

        List<GameObject> p1Offers = PickRandom(passiveItemDatabase.passiveItemPrefabs, 3);
        List<P2Card> p2Offers = PickRandom(cardDatabase.cards, 3);

        GameObject instance = Instantiate(levelUpRewardScreenPrefab);
        instance.GetComponent<LevelUpRewardScreenUI>().Setup(
            p1Offers,
            p2Offers,
            OnP1Chose,
            OnP2Chose
        );
    }

    void OnP1Chose(GameObject passivePrefab)
    {
        PlayerStats p1 = FindAnyObjectByType<PlayerStats>();
        if (p1 != null)
            p1.SpawnPassiveItem(passivePrefab);

        _rewardActive = false;
    }

    void OnP2Chose(P2Card card)
    {
        P2DeckManager deck = FindAnyObjectByType<P2DeckManager>();
        if (deck != null)
        {
            deck.AddCardToDeck(card);
            deck.AddCardToDeck(card); // ×2 copies
        }
    }

    static List<T> PickRandom<T>(List<T> source, int count)
    {
        List<T> pool = new List<T>(source);
        List<T> result = new List<T>();
        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return result;
    }
}