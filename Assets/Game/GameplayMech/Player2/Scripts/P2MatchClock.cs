using UnityEngine;

public sealed class P2MatchClock : MonoBehaviour
{
    [SerializeField] private bool useUnscaledTime;

    public float ElapsedSeconds { get; private set; }

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        ElapsedSeconds += dt;
    }

    public void ResetClock()
    {
        ElapsedSeconds = 0f;
    }
}