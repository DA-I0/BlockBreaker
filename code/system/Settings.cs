using Godot;

public enum InputType { gamepad, keyboard, mouse };

public class Settings
{
	private const string DefaultLanguage = "en";
	private const int DefaultInputType = 1;
	private const int DefaultScreenMode = 3;
	private const int DefaultScreenWidth = 1280;
	private const int DefaultScreenHeight = 1120;
	private const bool DefaultScreenShake = true;
	private const float DefaultBackgroundBrightness = 1f;
	private const float DefaultHelperTransparency = 0.4f;
	private const float DefaultMasterVolume = 0f;
	private const float DefaultMusicVolume = -10f;
	private const float DefaultEffectsVolume = -20f;
	private const float DefaultMouseSpeed = 1f;
	private const float DefaultKeyboardSpeed = 1f;

	private InputType _activeController = InputType.keyboard;
	private ConfigFile _config;
	private SessionController refs;

	public string Language
	{
		get { return (string)_config.GetValue("general", "language", DefaultLanguage); }
		set { _config.SetValue("general", "language", value); }
	}

	public int ScreenMode
	{
		get { return (int)_config.GetValue("display", "screen_mode", DefaultScreenMode); }
		set { _config.SetValue("display", "screen_mode", value); }
	}

	public int ScreenWidth
	{
		get { return (int)_config.GetValue("display", "screen_width", DefaultScreenWidth); }
		set { _config.SetValue("display", "screen_width", value); }
	}

	public int ScreenHeight
	{
		get { return (int)_config.GetValue("display", "screen_height", DefaultScreenHeight); }
		set { _config.SetValue("display", "screen_height", value); }
	}

	public bool ScreenShake
	{
		get { return (bool)_config.GetValue("display", "screen_shake", DefaultScreenShake); }
		set { _config.SetValue("display", "screen_shake", value); }
	}

	public float BackgroundBrightness
	{
		get { return (float)_config.GetValue("display", "background_brightness", DefaultBackgroundBrightness); }
		set { _config.SetValue("display", "background_brightness", value); }
	}

	public float HelperTransparency
	{
		get { return (float)_config.GetValue("display", "helper_transparency", DefaultHelperTransparency); }
		set { _config.SetValue("display", "helper_transparency", value); }
	}

	public float MasterVolume
	{
		get { return (float)_config.GetValue("audio", "master_volume", DefaultMasterVolume); }
		set { _config.SetValue("audio", "master_volume", value); }
	}

	public float MusicVolume
	{
		get { return (float)_config.GetValue("audio", "music_volume", DefaultMusicVolume); }
		set { _config.SetValue("audio", "music_volume", value); }
	}

	public float EffectsVolume
	{
		get { return (float)_config.GetValue("audio", "effects_volume", DefaultEffectsVolume); }
		set { _config.SetValue("audio", "effects_volume", value); }
	}

	public float SpeedMouse
	{
		get { return (float)_config.GetValue("general", "mouse_speed", DefaultMouseSpeed); }
		set { _config.SetValue("general", "mouse_speed", value); }
	}

	public float SpeedKeyboard
	{
		get { return (float)_config.GetValue("general", "keyboard_speed", DefaultKeyboardSpeed); }
		set { _config.SetValue("general", "keyboard_speed", value); }
	}

	public InputType ActiveController
	{
		get { return _activeController; }
		set
		{
			_activeController = value;
			_config.SetValue("general", "inputType", (int)value);
		}
	}

	public void SetDefaultValues()
	{
		Language = DefaultLanguage;
		ActiveController = (InputType)DefaultInputType;
		ScreenMode = DefaultScreenMode;
		ScreenWidth = DefaultScreenWidth;
		ScreenHeight = DefaultScreenHeight;
		ScreenShake = DefaultScreenShake;
		BackgroundBrightness = DefaultBackgroundBrightness;
		HelperTransparency = DefaultHelperTransparency;
		MasterVolume = DefaultMasterVolume;
		MusicVolume = DefaultMusicVolume;
		EffectsVolume = DefaultEffectsVolume;
		SpeedMouse = DefaultMouseSpeed;
		SpeedKeyboard = DefaultKeyboardSpeed;
	}

	public Settings(SessionController sessionController)
	{
		refs = sessionController;
		LoadSettings();
	}

	public void LoadSettings()
	{
		_config = new ConfigFile();
		Error error = _config.Load(refs.gameData.ConfigFilePath);

		if (error != Error.Ok)
		{
			SetDefaultValues();
		}

		ApplySettings();
	}

	public void SaveSettings()
	{
		_config.Save(refs.gameData.ConfigFilePath);
		ApplySettings();
	}

	private void ApplySettings()
	{
		TranslationServer.SetLocale(Language);

		DisplayServer.WindowSetMode((DisplayServer.WindowMode)ScreenMode, 0);
		Vector2I newResolution = new Vector2I(ScreenWidth, ScreenHeight);
		DisplayServer.WindowSetSize(newResolution, 0);

		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), MasterVolume);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), MusicVolume);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), EffectsVolume);
	}
}