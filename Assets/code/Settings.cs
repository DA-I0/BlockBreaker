using UnityEngine;

public class Settings : MonoBehaviour
{
	public string ConfigFilePath;

	[SerializeField] private bool _fullScreen = true;
	[SerializeField] private Resolution _resolution;

	[SerializeField] private float _volume = 0.5f;

	[SerializeField] private int _speedMouse = 10;
	[SerializeField] private int _speedKeyboard = 10;

	FileOperations fileOperations;

	public bool FullScreen
	{
		get { return _fullScreen; }
		set { _fullScreen = value; }
	}

	public Resolution Resolution
	{
		get { return _resolution; }
		set { _resolution = value; }
	}

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
		_fullScreen = true;
		_resolution = Screen.currentResolution;
		_volume = 0.5f;
		_speedMouse = 14;
		_speedKeyboard = 14;
	}

	public SettingsFile ExportSettings()
	{
		SettingsFile data = new SettingsFile();

		data.fullScreen = _fullScreen;
		data.screenWidth = _resolution.width;
		data.screenHeight = _resolution.height;
		data.volume = _volume;
		data.speedMouse = _speedMouse;
		data.speedKeyboard = _speedKeyboard;

		return data;
	}

	public void ImportSettings(SettingsFile data)
	{
		_fullScreen = data.fullScreen;
		_resolution = CreateNewResolution(data.screenWidth, data.screenHeight);
		_volume = data.volume;
		_speedMouse = data.speedMouse;
		_speedKeyboard = data.speedKeyboard;
		ApplySettings();
	}

	public void LoadSettings()
	{
		fileOperations.LoadSettings();
	}

	public void SaveSettings()
	{
		fileOperations.SaveSettings();
		ApplySettings();
	}

	public Resolution StringToResolution(string textResolution)
	{
		string[] resolutionParts = textResolution.Split(' ');

		Resolution newResolution = new Resolution();
		int tempValue = 0;
		int.TryParse(resolutionParts[0], out tempValue);
		newResolution.width = tempValue;
		int.TryParse(resolutionParts[2], out tempValue);
		newResolution.height = tempValue;
		int.TryParse(resolutionParts[4].Replace("Hz", ""), out tempValue);
		newResolution.refreshRate = tempValue;

		return newResolution;
	}

	private void Awake()
	{
		ConfigFilePath = Application.persistentDataPath + "/config.xml";
		fileOperations = new FileOperations(this);
		LoadSettings();
	}

	private void ApplySettings()
	{
		if (_fullScreen)
		{
			Screen.SetResolution(_resolution.width, _resolution.height, FullScreenMode.FullScreenWindow);
		}
		else
		{
			Screen.SetResolution(_resolution.width, _resolution.height, FullScreenMode.Windowed);
		}
	}

	private Resolution CreateNewResolution(int width, int height)
	{
		Resolution newResolution = new Resolution();
		newResolution.width = width;
		newResolution.height = height;
		newResolution.refreshRate = Screen.currentResolution.refreshRate;

		return newResolution;
	}
}