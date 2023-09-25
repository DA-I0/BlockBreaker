public class Settings
{
	private const string DefaultLanguage = "english";
	private const bool DefaultFullScreen = true;
	private const bool DefaultScreenShake = true;
	private const float DefaultMasterVolume = 1f;
	private const float DefaultMusicVolume = 0.2f;
	private const float DefaultEffectVolume = 0.15f;
	private const int DefaultMouseSpeed = 14;
	private const int DefaultKeyboardSpeed = 14;

	private string _language = "english";
	private bool _fullScreen = true;
	// private Resolution _resolution;
	private bool _screenShake = true;

	private float _masterVolume = 1f;
	private float _musicVolume = 0.5f;
	private float _effectVolume = 0.5f;

	private int _speedMouse = 10;
	private int _speedKeyboard = 10;

	private SessionController refs;

	public string Language
	{
		get { return _language; }
		set { _language = value; }
	}
	public bool FullScreen
	{
		get { return _fullScreen; }
		set { _fullScreen = value; }
	}

	public bool ScreenShake
	{
		get { return _screenShake; }
		set { _screenShake = value; }
	}

	// public Resolution CurrentResolution
	// {
	// 	get { return _resolution; }
	// 	set { _resolution = value; }
	// }

	public float MasterVolume
	{
		get { return _masterVolume; }
		set { _masterVolume = value; }
	}

	public float MusicVolume
	{
		get { return _musicVolume; }
		set { _musicVolume = value; }
	}

	public float EffectVolume
	{
		get { return _effectVolume; }
		set { _effectVolume = value; }
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
		_language = DefaultLanguage;
		_fullScreen = DefaultFullScreen;
		// _resolution = Displa.currentResolution;
		_screenShake = DefaultScreenShake;
		_masterVolume = DefaultMasterVolume;
		_musicVolume = DefaultMusicVolume;
		_effectVolume = DefaultEffectVolume;
		_speedMouse = DefaultMouseSpeed;
		_speedKeyboard = DefaultKeyboardSpeed;
	}

	public void LoadSettings()
	{
		//ProjectSettings.GetSetting("application/config/name").ToString();
		// _language = PlayerPrefs.GetString("Language", DefaultLanguage);
		// _fullScreen = (PlayerPrefs.GetInt("Screenmanager Fullscreen mode", (DefaultFullScreen ? 1 : 3)) < 2);
		// _screenShake = (PlayerPrefs.GetInt("Screen shake", (DefaultScreenShake ? 1 : 0)) > 0);
		// _resolution = CreateNewResolution(PlayerPrefs.GetInt("Screenmanager Resolution Width"), PlayerPrefs.GetInt("Screenmanager Resolution Height"));
		// _masterVolume = PlayerPrefs.GetFloat("Master volume", DefaultMasterVolume);
		// _musicVolume = PlayerPrefs.GetFloat("Music volume", DefaultMusicVolume);
		// _effectVolume = PlayerPrefs.GetFloat("Effect volume", DefaultEffectVolume);
		// _speedMouse = PlayerPrefs.GetInt("Input Mouse Speed", DefaultMouseSpeed);
		// _speedKeyboard = PlayerPrefs.GetInt("Input Keyboard Speed", DefaultKeyboardSpeed);
	}

	public void SaveSettings()
	{
		// PlayerPrefs.SetString("Language", (_language));
		// PlayerPrefs.SetInt("Screenmanager Fullscreen mode", (_fullScreen ? 1 : 3));
		// PlayerPrefs.SetInt("Screenmanager Resolution Width", _resolution.width);
		// PlayerPrefs.SetInt("Screenmanager Resolution Height", _resolution.height);
		// PlayerPrefs.SetInt("Screen shake", (_screenShake ? 1 : 0));
		// PlayerPrefs.SetFloat("Master volume", _masterVolume);
		// PlayerPrefs.SetFloat("Music volume", _musicVolume);
		// PlayerPrefs.SetFloat("Effect volume", _effectVolume);
		// PlayerPrefs.SetInt("Input Mouse Speed", _speedMouse);
		// PlayerPrefs.SetInt("Input Keyboard Speed", _speedKeyboard);

		// PlayerPrefs.Save();
		ApplySettings();
	}

	// public Resolution StringToResolution(string textResolution)
	// {
	// 	string[] resolutionParts = textResolution.Split(' ');

	// 	Resolution newResolution = new Resolution();
	// 	int tempValue = 0;
	// 	int.TryParse(resolutionParts[0], out tempValue);
	// 	newResolution.width = tempValue;
	// 	int.TryParse(resolutionParts[2], out tempValue);
	// 	newResolution.height = tempValue;
	// 	int.TryParse(resolutionParts[4].Replace("Hz", ""), out tempValue);
	// 	newResolution.refreshRate = tempValue;

	// 	return newResolution;
	// }

	private void Awake()
	{
		// refs = gameObject.GetComponent<CommonRefs>();
		LoadSettings();
	}

	private void ApplySettings()
	{
		// if (_fullScreen)
		// {
		// 	Screen.SetResolution(_resolution.width, _resolution.height, FullScreenMode.FullScreenWindow);
		// }
		// else
		// {
		// 	Screen.SetResolution(_resolution.width, _resolution.height, FullScreenMode.Windowed);
		// }

		// BroadcastMessage("UpdateVolume");
		// refs.gameData.LoadLocalization();
	}

	// private Resolution CreateNewResolution(int width, int height)
	// {
	// 	Resolution newResolution = new Resolution();
	// 	newResolution.width = width;
	// 	newResolution.height = height;
	// 	newResolution.refreshRate = Screen.currentResolution.refreshRate;

	// 	return newResolution;
	// }
}