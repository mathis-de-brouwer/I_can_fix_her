using System.Collections.Generic;
using UnityEngine;

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

        SetP1GameplayInputEnabled(false);

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

    void SetP1GameplayInputEnabled(bool enabled)
    {
        Playermovement movement = FindAnyObjectByType<Playermovement>();
        if (movement != null)
        {
            movement.InputLocked = !enabled;

            if (enabled) movement.playerControlls.Enable();
            else movement.playerControlls.Disable();

            if (enabled) movement.dashAction.Enable();
            else movement.dashAction.Disable();
        }

        AimController aim = FindAnyObjectByType<AimController>();
        if (aim != null)
        {
            if (enabled) aim.aimAction.Enable();
            else aim.aimAction.Disable();

            if (enabled) aim.aimUp.Enable();
            else aim.aimUp.Disable();

            if (enabled) aim.aimDown.Enable();
            else aim.aimDown.Disable();

            if (enabled) aim.aimLeft.Enable();
            else aim.aimLeft.Disable();

            if (enabled) aim.aimRight.Enable();
            else aim.aimRight.Disable();
        }
    }

    public void OnRewardScreenClosed()
    {
        Time.timeScale = 1f;
        SetP1GameplayInputEnabled(true);
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