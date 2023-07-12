using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
	[SerializeField] private Slider _volume;
	[SerializeField] private Slider _speedMouse;
	[SerializeField] private Slider _speedKeyboard;

	private Settings _settings;

	public void UpdateUI()
	{
		_volume.value = _settings.Volume;
		_speedMouse.value = _settings.SpeedMouse;
		_speedKeyboard.value = _settings.SpeedKeyboard;
	}

	public void SetValues()
	{
		_settings.Volume = _volume.value;
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
	}
}
