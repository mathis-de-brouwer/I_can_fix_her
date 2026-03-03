using UnityEngine;

public sealed class UiSfx : MonoBehaviour
{
	private static UiSfx _instance;

	[SerializeField] private UnityEngine.AudioSource sfxSource;

	[Header("UI SFX clips")]
	[SerializeField] private AudioClip sfx1Confirm;
	[SerializeField] private AudioClip sfx2DeckbuilderCard;
	[SerializeField] private AudioClip sfx3GameplayCard;
	[SerializeField] private AudioClip sfx4RewardChoice;

	[Header("Gameplay SFX clips")]
	[SerializeField] private AudioClip sfxEnemyHit;
	[SerializeField] private AudioClip sfxPlayerHit;
	[SerializeField] private AudioClip sfxEnemyDeath;
	[SerializeField] private AudioClip sfxPlayerDeath;

	[Header("Weapon SFX clips")]
	[SerializeField] private AudioClip sfxKnifeThrow;
	[SerializeField] private AudioClip sfxOrbitSpawn;
	[SerializeField] private AudioClip sfxShieldBreak;
	[SerializeField] private AudioClip sfxShieldRegen;

	[Header("Gameplay SFX anti-spam")]
	[SerializeField, Min(0f)] private float enemyHitCooldownSeconds = 0.05f;
	[SerializeField, Min(0f)] private float playerHitCooldownSeconds = 0.15f;

	private float _nextEnemyHitTime;
	private float _nextPlayerHitTime;

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

	public static void PlayClick() => PlayConfirm();

	public static void PlayConfirm()
		=> Play(_instance != null ? _instance.sfx1Confirm : null);

	public static void PlayDeckbuilderCard()
		=> Play(_instance != null ? _instance.sfx2DeckbuilderCard : null);

	public static void PlayGameplayCard()
		=> Play(_instance != null ? _instance.sfx3GameplayCard : null);

	public static void PlayRewardChoice()
		=> Play(_instance != null ? _instance.sfx4RewardChoice : null);

	public static void PlayEnemyHit()
	{
		if (_instance == null)
			return;

		if (Time.unscaledTime < _instance._nextEnemyHitTime)
			return;

		_instance._nextEnemyHitTime = Time.unscaledTime + _instance.enemyHitCooldownSeconds;
		Play(_instance.sfxEnemyHit);
	}

	public static void PlayPlayerHit()
	{
		if (_instance == null)
			return;

		if (Time.unscaledTime < _instance._nextPlayerHitTime)
			return;

		_instance._nextPlayerHitTime = Time.unscaledTime + _instance.playerHitCooldownSeconds;
		Play(_instance.sfxPlayerHit);
	}

	public static void PlayEnemyDeath()
		=> Play(_instance != null ? _instance.sfxEnemyDeath : null);

	public static void PlayPlayerDeath()
		=> Play(_instance != null ? _instance.sfxPlayerDeath : null);

	public static void PlayKnifeThrow()
		=> Play(_instance != null ? _instance.sfxKnifeThrow : null);

	public static void PlayOrbitSpawn()
		=> Play(_instance != null ? _instance.sfxOrbitSpawn : null);

	public static void PlayShieldBreak()
		=> Play(_instance != null ? _instance.sfxShieldBreak : null);

	public static void PlayShieldRegen()
		=> Play(_instance != null ? _instance.sfxShieldRegen : null);

	private static void Play(AudioClip clip)
	{
		if (_instance == null)
			return;

		if (_instance.sfxSource == null || clip == null)
			return;

		_instance.sfxSource.PlayOneShot(clip);
	}
}