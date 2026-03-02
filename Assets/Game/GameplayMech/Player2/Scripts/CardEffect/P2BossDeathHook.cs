using UnityEngine;

public sealed class P2BossDeathHook : MonoBehaviour
{
    private P2DeckManager _deckManager;

    public void Initialize(P2DeckManager deckManager)
    {
        _deckManager = deckManager;
    }

    private void OnDestroy()
    {
        if (_deckManager != null)
            _deckManager.NotifyBossDied();
    }
}