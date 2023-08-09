using UnityEngine;

public class AudioController : MonoBehaviour
{
	[SerializeField] private AudioClip _menuMusic;
	[SerializeField] private AudioClip _stageMusic;
	[SerializeField] private AudioClip[] _sounds;

	private AudioSource _musicSource;
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
}