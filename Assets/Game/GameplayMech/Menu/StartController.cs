using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    public void OnStartClick()
    {
         SceneManager.LoadScene("SampleScene");
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
