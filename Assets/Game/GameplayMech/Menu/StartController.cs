using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string startSceneName = "SampleScene";

    private void Start()
    {
        MusicService.Play(MusicId.Menu);
    }

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

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
