using UnityEngine;

public class Settings : MonoBehaviour
{
	public string ConfigFilePath;

	[SerializeField] private float _volume = 0.5f;

	[SerializeField] private int _speedMouse = 10;
	[SerializeField] private int _speedKeyboard = 10;

	FileOperations fileOperations;

	public float Volume
	{
		get { return _volume; }
		set { _volume = value; }
	}

	public int SpeedMouse
	{
		get { return _speedMouse; }
		set { _speedMouse = value; }
	}

	public int SpeedKeyboard
	{
		get { return _speedKeyboard; }
		set { _speedKeyboard = value; }
	}

	public void SetDefaultValues()
	{
		_volume = 0.5f;
		_speedMouse = 14;
		_speedKeyboard = 14;
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