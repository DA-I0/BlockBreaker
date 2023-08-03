using UnityEngine;

public class AudioController : MonoBehaviour
{
	[SerializeField] private AudioClip _menuMusic;
	[SerializeField] private AudioClip _stageMusic;
	[SerializeField] private AudioClip[] _sounds;

	private float _currentMusicVolume = 0;
	private float _currentEffectVolume = 0;

	private AudioSource _soundSource;
	private Settings _settings;

	// NOTE: temp types
	// 0 - menu
	// 1 - level
	public void ChangeSong(int type)
	{
		if (type == 0)
		{
			PlayMusic(_menuMusic);
		}
		else
		{
			PlayMusic(_stageMusic);
		}
	}

	public void PlaySound(int type)
	{
		_soundSource.PlayOneShot(_sounds[type], _currentEffectVolume);
	}

	public void UpdateVolume()
	{
		_currentMusicVolume = GetVolumeValue(true);
		_currentEffectVolume = GetVolumeValue();
		_soundSource.volume = _currentMusicVolume;
	}

	private void Awake()
	{
		_soundSource = gameObject.GetComponent<AudioSource>();
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
		if (_soundSource == null)
		{
			return;
		}

		_soundSource.volume = _currentMusicVolume;
		_soundSource.loop = true;
		_soundSource.clip = song;
		_soundSource.Play();
	}
}