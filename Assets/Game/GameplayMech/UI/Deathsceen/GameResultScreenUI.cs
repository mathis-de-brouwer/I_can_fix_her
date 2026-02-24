using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        Time.timeScale = 0f;
    }

    public void OnContinueClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}