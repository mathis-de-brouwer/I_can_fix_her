using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string startSceneName = "SampleScene";

    public void OnStartClick()
    {
        UiSfx.PlayClick();

        if (string.IsNullOrWhiteSpace(startSceneName))
        {
            Debug.LogError($"{nameof(StartController)}: Start scene name is empty.");
            return;
        }

        if (MatchStats.Instance != null)
            MatchStats.Instance.ResetAll();

        SceneManager.LoadScene(startSceneName);
    }

    public void OnExitClick()
    {
        UiSfx.PlayClick();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
