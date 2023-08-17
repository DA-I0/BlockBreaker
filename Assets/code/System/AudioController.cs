using UnityEngine;

public class AudioController : MonoBehaviour
{
	[SerializeField] private AudioClip _menuMusic;
	[SerializeField] private AudioClip[] _stageMusic;
	[SerializeField] private AudioClip[] _sounds;

	private AudioSource _musicSource;
	private AudioSource _soundSource;
	private Settings _settings;

	// NOTE: temp types
	// 0 - menu
	// 1 - level
	// TODO: move songID/Index to a new "LevelInfo" class that will
	// also include level name, other stuff? Defaults to -1 (randomize)
	// This class should be responsible for changing song on load.
	public void ChangeSong(int type, int songIndex = -1)
	{
		AudioClip songToPlay;

		if (type == 0)
		{
			songToPlay = _menuMusic;
		}
		else
		{
			songToPlay = (songIndex > 0) ? _stageMusic[songIndex] : RandomizeSong(ref _stageMusic);
		}

		PlayMusic(songToPlay);
	}

	public void PlaySound(int type)
	{
		_soundSource.PlayOneShot(_sounds[type], 1f);
	}

	public void UpdateVolume()
	{
		_musicSource.volume = GetVolumeValue(true);
		_soundSource.volume = GetVolumeValue();
	}

	private void Awake()
	{
		_musicSource = gameObject.GetComponent<AudioSource>();
		_soundSource = GameObject.Find("sfx_source").GetComponent<AudioSource>();
		_settings = gameObject.GetComponent<Settings>();
	}

	private void Start()
	{
		UpdateVolume();
	}

	private float GetVolumeValue(bool music = false)
	{
		float newVolume = music ? _settings.MusicVolume : _settings.EffectVolume;
		return newVolume * _settings.MasterVolume;
	}

	private void PlayMusic(AudioClip song)
	{
		if (_musicSource == null)
		{
			return;
		}

		_musicSource.loop = true;
		_musicSource.clip = song;
		_musicSource.Play();
	}

	private AudioClip RandomizeSong(ref AudioClip[] sourceArray)
	{
		int songIndex = Random.Range(0, sourceArray.Length);
		return sourceArray[songIndex];
	}
}