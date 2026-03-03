using UnityEngine;

public sealed class UiSfx : MonoBehaviour
{
	private static UiSfx _instance;

	[SerializeField] private UnityEngine.AudioSource sfxSource;

	[Header("SFX clips")]
	[SerializeField] private AudioClip sfx1Confirm;
	[SerializeField] private AudioClip sfx2DeckbuilderCard;
	[SerializeField] private AudioClip sfx3GameplayCard;
	[SerializeField] private AudioClip sfx4RewardChoice;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	// Backwards-compatible alias for older call sites.
	public static void PlayClick()
		=> PlayConfirm();

	public static void PlayConfirm()
		=> Play(_instance != null ? _instance.sfx1Confirm : null);

	public static void PlayDeckbuilderCard()
		=> Play(_instance != null ? _instance.sfx2DeckbuilderCard : null);

	public static void PlayGameplayCard()
		=> Play(_instance != null ? _instance.sfx3GameplayCard : null);

	public static void PlayRewardChoice()
		=> Play(_instance != null ? _instance.sfx4RewardChoice : null);

	private static void Play(AudioClip clip)
	{
		if (_instance == null)
			return;

		if (_instance.sfxSource == null || clip == null)
			return;

		_instance.sfxSource.PlayOneShot(clip);
	}
}