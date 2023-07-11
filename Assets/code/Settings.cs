using UnityEngine;

public class Settings : MonoBehaviour
{
	public string ConfigFilePath;

	[SerializeField] private float _volume = 0.5f;

	[SerializeField] private float _speedMouse = 1.0f;
	[SerializeField] private float _speedKeyboard = 1.0f;

	FileOperations fileOperations;

	public float Volume
	{
		get { return _volume; }
		set { _volume = value; }
	}

	public float SpeedMouse
	{
		get { return _speedMouse; }
		set { _speedMouse = value; }
	}

	public float SpeedKeyboard
	{
		get { return _speedKeyboard; }
		set { _speedKeyboard = value; }
	}

	public void SetDefaultValues()
	{
		_volume = 0.5f;
		_speedMouse = 1.0f;
		_speedKeyboard = 1.0f;
	}

	public SettingsFile ExportSettings()
	{
		SettingsFile data = new SettingsFile();

		data.volume = _volume;
		data.speedMouse = _speedMouse;
		data.speedKeyboard = _speedKeyboard;

		return data;
	}

	public void ImportSettings(SettingsFile settingData)
	{
		_volume = settingData.volume;
		_speedMouse = settingData.speedMouse;
		_speedKeyboard = settingData.speedKeyboard;
	}

	public void LoadSettings()
	{
		fileOperations.LoadSettings();
	}

	public void SaveSettings()
	{
		fileOperations.SaveSettings();
	}

	private void Awake()
	{
		ConfigFilePath = Application.persistentDataPath + "/config.xml";
		fileOperations = new FileOperations(this);
		LoadSettings();
	}
}