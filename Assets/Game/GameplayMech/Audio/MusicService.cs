using UnityEngine;

public sealed class MusicService : MonoBehaviour
{
	private static MusicService _instance;

	[SerializeField] private AudioSource musicSource;

	[Header("Music clips")]
	[SerializeField] private AudioClip menuMusic;
	[SerializeField] private AudioClip deckBuilderMusic;
	[SerializeField] private AudioClip gameplayMusic;
	[SerializeField] private AudioClip levelUpMusic;
	[SerializeField] private AudioClip gameResultMusic;

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

	public static void Play(MusicId id)
	{
		if (_instance == null)
			return;

		switch (id)
		{
			case MusicId.Menu:
				PlayMusic(_instance.menuMusic, true);
				break;

			case MusicId.DeckBuilder:
				PlayMusic(_instance.deckBuilderMusic, true);
				break;

			case MusicId.Gameplay:
				PlayMusic(_instance.gameplayMusic, true);
				break;

			case MusicId.LevelUp:
				PlayMusic(_instance.levelUpMusic, true);
				break;

			case MusicId.GameResult:
				PlayMusic(_instance.gameResultMusic, true);
				break;

			default:
				PlayMusic(null, false);
				break;
		}
	}

	private static void PlayMusic(AudioClip clip, bool loop)
	{
		if (_instance == null || _instance.musicSource == null)
			return;

		if (_instance.musicSource.clip == clip && _instance.musicSource.isPlaying)
			return;

		_instance.musicSource.loop = loop;
		_instance.musicSource.clip = clip;

		if (clip != null)
			_instance.musicSource.Play();
		else
			_instance.musicSource.Stop();
	}
}