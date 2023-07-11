using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
	[SerializeField] private Scrollbar _volume;
	[SerializeField] private Scrollbar _speedMouse;
	[SerializeField] private Scrollbar _speedKeyboard;

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
		_settings.SpeedMouse = _speedMouse.value;
		_settings.SpeedKeyboard = _speedKeyboard.value;
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
