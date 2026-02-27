using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Instantiated at runtime by PlayerStats (P1 death) or P2DeckManager (deck empty).
/// No scene references needed — Setup() is called right after Instantiate().
/// </summary>
public class GameResultScreenUI : MonoBehaviour
{
    public enum Winner { P1, P2 }

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("P1 result panel (left side of screen)")]
    [SerializeField] private GameObject p1ResultPanel;
    [SerializeField] private TMP_Text p1ResultText;

    [Header("P2 result panel (right side of screen)")]
    [SerializeField] private GameObject p2ResultPanel;
    [SerializeField] private TMP_Text p2ResultText;

    [Header("Identity visuals")]
    [SerializeField] private Image p1PortraitImage;
    [SerializeField] private Image p2CardBackImage;
    [SerializeField] private Sprite p2CardBackSprite;

    [Header("P1 stats (optional UI fields)")]
    [SerializeField] private TMP_Text p1KillsText;
    [SerializeField] private TMP_Text p1LoadoutText;
    [SerializeField] private TMP_Text p1ScoreText;

    [Header("P2 stats (optional UI fields)")]
    [SerializeField] private TMP_Text p2DamageText;
    [SerializeField] private TMP_Text p2ScoreText;

    [Header("P2 cards used (icons)")]
    [SerializeField] private RectTransform p2CardsUsedContainer;
    [SerializeField] private ResultIconEntryUI p2CardIconEntryPrefab;

    /// <summary>
    /// Called immediately after Instantiate() to configure the screen.
    /// </summary>
    public void Setup(Winner winner)
    {
        if (p1ResultText != null)
            p1ResultText.text = winner == Winner.P1 ? "You Win!" : "You Died";

        if (p2ResultText != null)
            p2ResultText.text = winner == Winner.P2 ? "You Win!" : "You Lost";

        if (p1ResultPanel != null) p1ResultPanel.SetActive(true);
        if (p2ResultPanel != null) p2ResultPanel.SetActive(true);

        PopulateStatsAndVisuals();

        Time.timeScale = 0f;
    }

    private void PopulateStatsAndVisuals()
    {
        int kills = MatchStats.Instance != null ? MatchStats.Instance.P1MonstersKilled : 0;
        float damageToP1 = MatchStats.Instance != null ? MatchStats.Instance.P2DamageDoneToP1 : 0f;
        float elapsed = MatchStats.Instance != null ? MatchStats.Instance.ElapsedSeconds : 0f;

        if (p1KillsText != null)
            p1KillsText.text = $"Kills: {kills}";

        PlayerStats p1 = FindAnyObjectByType<PlayerStats>();
        InventoryManager inventory = p1 != null ? p1.GetComponent<InventoryManager>() : FindAnyObjectByType<InventoryManager>();

        if (p1LoadoutText != null)
            p1LoadoutText.text = BuildP1LoadoutText(inventory);

        SetIdentitySprites(p1);

        P2DeckManager deck = FindAnyObjectByType<P2DeckManager>();
        RenderP2CardsUsed(deck);

        if (p2DamageText != null)
            p2DamageText.text = $"Damage to P1: {damageToP1:0}";

        int weaponCount = CountNonNullWeapons(inventory);
        int passiveCount = CountNonNullPassives(inventory);
        int totalCardsUsed = deck != null && deck.usedPile != null ? deck.usedPile.Count : 0;

        int p1Score = ComputeP1Score(kills, weaponCount, passiveCount, elapsed);
        int p2Score = ComputeP2Score(damageToP1, totalCardsUsed);

        if (p1ScoreText != null)
            p1ScoreText.text = $"Score: {p1Score}";

        if (p2ScoreText != null)
            p2ScoreText.text = $"Score: {p2Score}";
    }

    private void SetIdentitySprites(PlayerStats p1)
    {
        if (p1PortraitImage != null)
        {
            Sprite p1Sprite = null;

            // Prefer a configured sprite on characterData if you have one; otherwise fall back to SpriteRenderer.
            if (p1 != null && p1.TryGetComponent(out SpriteRenderer sr) && sr != null)
                p1Sprite = sr.sprite;

            p1PortraitImage.sprite = p1Sprite;
            p1PortraitImage.enabled = p1Sprite != null;
        }

        if (p2CardBackImage != null)
        {
            p2CardBackImage.sprite = p2CardBackSprite;
            p2CardBackImage.enabled = p2CardBackSprite != null;
        }
    }

    private void RenderP2CardsUsed(P2DeckManager deck)
    {
        if (p2CardsUsedContainer == null || p2CardIconEntryPrefab == null)
            return;

        for (int i = p2CardsUsedContainer.childCount - 1; i >= 0; i--)
            Destroy(p2CardsUsedContainer.GetChild(i).gameObject);

        if (deck == null || deck.usedPile == null)
        {
            Debug.LogWarning("GameResult: deck/usedPile is null.");
            return;
        }

        List<P2Card> cards = deck.usedPile;

        // Optional: group same cards together (keeps duplicates adjacent).
        cards.Sort((a, b) =>
        {
            string an = a != null ? (string.IsNullOrWhiteSpace(a.cardName) ? a.name : a.cardName) : string.Empty;
            string bn = b != null ? (string.IsNullOrWhiteSpace(b.cardName) ? b.name : b.cardName) : string.Empty;
            return string.CompareOrdinal(an, bn);
        });

        for (int i = 0; i < cards.Count; i++)
        {
            P2Card card = cards[i];
            if (card == null)
                continue;

            Sprite sprite = GetCardSprite(card);
            if (sprite == null)
                continue;

            ResultIconEntryUI entry = Instantiate(p2CardIconEntryPrefab, p2CardsUsedContainer);

            // duplicates are now represented by multiple entries, so count is always 1
            entry.Setup(sprite, 1);
        }
    }

    private static Dictionary<Sprite, int> BuildP2CardSpriteCounts(P2DeckManager deck)
    {
        Dictionary<Sprite, int> counts = new Dictionary<Sprite, int>();
        if (deck == null || deck.usedPile == null)
            return counts;

        for (int i = 0; i < deck.usedPile.Count; i++)
        {
            P2Card card = deck.usedPile[i];
            if (card == null)
                continue;

            Sprite sprite = GetCardSprite(card);
            if (sprite == null)
                continue;

            if (counts.TryGetValue(sprite, out int existing))
                counts[sprite] = existing + 1;
            else
                counts.Add(sprite, 1);
        }

        return counts;
    }

    private static Sprite GetCardSprite(P2Card card)
    {
        if (card == null)
            return null;

        if (card.effect != null && card.effect.CardArtOverride != null)
            return card.effect.CardArtOverride;

        return card.icon;
    }

    private static int ComputeP1Score(int kills, int weaponCount, int passiveCount, float elapsedSeconds)
    {
        int survivalBonus = Mathf.FloorToInt(elapsedSeconds * 2f);
        return (kills * 10) + (weaponCount * 100) + (passiveCount * 75) + survivalBonus;
    }

    private static int ComputeP2Score(float damageDoneToP1, int totalCardsPlayed)
    {
        int damageScore = Mathf.FloorToInt(damageDoneToP1 * 5f);
        return damageScore + (totalCardsPlayed * 20);
    }

    private static string BuildP1LoadoutText(InventoryManager inventory)
    {
        if (inventory == null)
            return "Loadout: (unknown)";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Weapons:");
        if (inventory.WeaponSlots != null)
        {
            for (int i = 0; i < inventory.WeaponSlots.Count; i++)
            {
                WeaponController wc = inventory.WeaponSlots[i];
                if (wc == null || wc.weaponData == null)
                    continue;

                string name = string.IsNullOrWhiteSpace(wc.weaponData.WeaponName) ? wc.weaponData.name : wc.weaponData.WeaponName;
                sb.AppendLine($"- {name} (Lv {wc.weaponData.Level})");
            }
        }

        sb.AppendLine();
        sb.AppendLine("Passives:");
        if (inventory.PassiveItemSlots != null)
        {
            for (int i = 0; i < inventory.PassiveItemSlots.Count; i++)
            {
                PassiveItems pi = inventory.PassiveItemSlots[i];
                if (pi == null || pi.passiveItemsData == null)
                    continue;

                string name = string.IsNullOrWhiteSpace(pi.passiveItemsData.ItemName) ? pi.passiveItemsData.name : pi.passiveItemsData.ItemName;
                sb.AppendLine($"- {name} (Lv {pi.passiveItemsData.Level})");
            }
        }

        return sb.ToString();
    }

    private static int CountNonNullWeapons(InventoryManager inventory)
    {
        if (inventory == null || inventory.WeaponSlots == null)
            return 0;

        int count = 0;
        for (int i = 0; i < inventory.WeaponSlots.Count; i++)
            if (inventory.WeaponSlots[i] != null)
                count++;

        return count;
    }

    private static int CountNonNullPassives(InventoryManager inventory)
    {
        if (inventory == null || inventory.PassiveItemSlots == null)
            return 0;

        int count = 0;
        for (int i = 0; i < inventory.PassiveItemSlots.Count; i++)
            if (inventory.PassiveItemSlots[i] != null)
                count++;

        return count;
    }

    public void OnContinueClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}