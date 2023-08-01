using UnityEngine;

public class Settings : MonoBehaviour
{
	private const bool DefaultFullScreen = true;
	private const float DefaultMasterVolume = 0.5f;
	private const int DefaultMouseSpeed = 14;
	private const int DefaultKeyboardSpeed = 14;

	[SerializeField] private bool _fullScreen = true;
	[SerializeField] private Resolution _resolution;

	[SerializeField] private float _volume = 0.5f;

	[SerializeField] private int _speedMouse = 10;
	[SerializeField] private int _speedKeyboard = 10;

	public GameData gameData;
	private FileOperations fileOperations;

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
		_fullScreen = DefaultFullScreen;
		_resolution = Screen.currentResolution;
		_volume = DefaultMasterVolume;
		_speedMouse = DefaultMouseSpeed;
		_speedKeyboard = DefaultKeyboardSpeed;
	}

	public void LoadSettings()
	{
		_fullScreen = PlayerPrefs.GetInt("Screenmanager Fullscreen mode") > 0;
		_resolution = CreateNewResolution(PlayerPrefs.GetInt("Screenmanager Resolution Width"), PlayerPrefs.GetInt("Screenmanager Resolution Height"));
		_volume = PlayerPrefs.GetFloat("Master volume", DefaultMasterVolume);
		_speedMouse = PlayerPrefs.GetInt("Input Mouse Speed", DefaultMouseSpeed);
		_speedKeyboard = PlayerPrefs.GetInt("Input Keyboard Speed", DefaultKeyboardSpeed);
	}

	public void SaveSettings()
	{
		PlayerPrefs.SetInt("Screenmanager Fullscreen mode", (_fullScreen ? 1 : 0));
		PlayerPrefs.SetInt("Screenmanager Resolution Width", _resolution.width);
		PlayerPrefs.SetInt("Screenmanager Resolution Height", _resolution.height);
		PlayerPrefs.SetFloat("Master volume", _volume);
		PlayerPrefs.SetInt("Input Mouse Speed", _speedMouse);
		PlayerPrefs.SetInt("Input Keyboard Speed", _speedKeyboard);

		PlayerPrefs.Save();
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
		string localConfigPath = Application.persistentDataPath + "/config.xml";
		string localChangelogPath = Application.dataPath + "/patchnotes.txt";

		gameData = new GameData(localConfigPath, localChangelogPath);
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