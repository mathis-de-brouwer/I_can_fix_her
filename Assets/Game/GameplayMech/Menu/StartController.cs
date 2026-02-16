using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string startSceneName = "SampleScene";

    public void OnStartClick()
    {
        if (string.IsNullOrWhiteSpace(startSceneName))
        {
            Debug.LogError($"{nameof(StartController)}: Start scene name is empty.");
            return;
        }

        SceneManager.LoadScene(startSceneName);
    }

    public void OnExitClick()
    {
        // has to be changed later when we make an executable of the game for the time remaining we just gonna close unity

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        //Application.Quit(); // For later when we have an executable of the game 
    }
}
