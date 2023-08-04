using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
	[SerializeField] private Toggle _fullscreen;
	[SerializeField] private TMP_Dropdown _resolution;
	[SerializeField] private Slider _masterVolume;
	[SerializeField] private Slider _musicVolume;
	[SerializeField] private Slider _effectVolume;
	[SerializeField] private Slider _speedMouse;
	[SerializeField] private Slider _speedKeyboard;

	private Settings _settings;

	public void UpdateUI()
	{
		_fullscreen.isOn = _settings.FullScreen;
		_resolution.value = FindResolution(_settings.CurrentResolution);
		_masterVolume.value = _settings.MasterVolume;
		_musicVolume.value = _settings.MusicVolume;
		_effectVolume.value = _settings.EffectVolume;
		_speedMouse.value = _settings.SpeedMouse;
		_speedKeyboard.value = _settings.SpeedKeyboard;
	}

	public void SetValues()
	{
		_settings.FullScreen = _fullscreen.isOn;
		_settings.CurrentResolution = _settings.StringToResolution(_resolution.options[_resolution.value].text);
		_settings.MasterVolume = _masterVolume.value;
		_settings.MusicVolume = _musicVolume.value;
		_settings.EffectVolume = _effectVolume.value;
		_settings.SpeedMouse = (int)_speedMouse.value;
		_settings.SpeedKeyboard = (int)_speedKeyboard.value;
		_settings.SaveSettings();
	}

	public void SetDefaultValues()
	{
		_settings.SetDefaultValues();
		UpdateUI();
	}

	private void Awake()
	{
		_settings = GameObject.Find("_system").GetComponent<Settings>();
		PopulateResolutionList();
	}

	private void PopulateResolutionList()
	{
		_resolution.options.Clear();

		foreach (Resolution res in Screen.resolutions)
		{
			_resolution.options.Add(new TMP_Dropdown.OptionData(res.ToString()));
		}
	}

	private int FindResolution(Resolution target)
	{
		string targetResolution = target.ToString();
		int index = 0;

		foreach (TMP_Dropdown.OptionData resolutionOption in _resolution.options)
		{
			if (targetResolution == resolutionOption.text)
			{
				_resolution.value = index;
				break;
			}

			index++;
		}

		return index;
	}
}
