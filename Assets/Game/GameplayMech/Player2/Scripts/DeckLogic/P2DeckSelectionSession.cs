using UnityEngine;

public sealed class P2DeckSelectionSession : MonoBehaviour
{
    public static P2DeckSelectionSession Instance { get; private set; }

    public P2DeckBuildConfig Config { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetConfig(P2DeckBuildConfig config)
    {
        Config = config;
    }
}