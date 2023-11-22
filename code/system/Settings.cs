using System.Collections.Generic;
using System.Linq;
using Godot;

public enum InputType { gamepad, keyboard, mouse };

public class Settings
{
	public bool firstLaunch = false;

	private const string DefaultLanguage = "en";
	private const string DefaultControlerPrompts = "generic";
	private const int DefaultInputType = 1;
	private const int DefaultScreenMode = 3;
	private const int DefaultScreenWidth = 1280;
	private const int DefaultScreenHeight = 1120;
	private const bool DefaultScreenShake = true;
	private const float DefaultBackgroundBrightness = 1f;
	private const int DefaultPickupOrder = 0;
	private const float DefaultHelperTransparency = 0.4f;
	private const float DefaultMasterVolume = 0f;
	private const float DefaultMusicVolume = -10f;
	private const float DefaultEffectsVolume = -20f;
	private const float DefaultMouseSpeed = 1f;
	private const float DefaultKeyboardSpeed = 1f;
	private const float DefaultJoypadSpeed = 1f;
	private Dictionary<string, Godot.Collections.Array<InputEvent>> DefaultKeybindings = new Dictionary<string, Godot.Collections.Array<InputEvent>>();

	private InputType _activeControllerType = InputType.keyboard;
	private int _activeJoypadId = 0;
	private ConfigFile _config;
	private SessionController refs;

	public string Language
	{
		get { return (string)_config.GetValue("general", "language", DefaultLanguage); }
		set { _config.SetValue("general", "language", value); }
	}

	public string ControlerPrompts
	{
		get { return (string)_config.GetValue("general", "controlerPrompts", DefaultControlerPrompts); }
		set { _config.SetValue("general", "controlerPrompts", value); }
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

	public int PickupOrder
	{
		get { return (int)_config.GetValue("display", "pickup_order", DefaultPickupOrder); }
		set { _config.SetValue("display", "pickup_order", value); }
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
		get { return (float)_config.GetValue("controls", "mouse_speed", DefaultMouseSpeed); }
		set { _config.SetValue("controls", "mouse_speed", value); }
	}

	public float SpeedKeyboard
	{
		get { return (float)_config.GetValue("controls", "keyboard_speed", DefaultKeyboardSpeed); }
		set { _config.SetValue("controls", "keyboard_speed", value); }
	}

	public float SpeedJoypad
	{
		get { return (float)_config.GetValue("controls", "joypad_speed", DefaultJoypadSpeed); }
		set { _config.SetValue("controls", "joypad_speed", value); }
	}

	public InputType ActiveController
	{
		get { return _activeControllerType; }
		set
		{
			_activeControllerType = value;
			_config.SetValue("general", "inputType", (int)value);
		}
	}

	public string ActiveJoypad
	{
		get { return (string)_config.GetValue("controls", "active_gamepad", string.Empty); }
		set { _config.SetValue("controls", "active_gamepad", value); }
	}

	public int ActiveJoypadID
	{
		get { return _activeJoypadId; }
		set { _activeJoypadId = value; }
	}

	public Settings(SessionController sessionController)
	{
		refs = sessionController;
		PrepareDefaultKeybindings();
		LoadSettings();
	}

	private void PrepareDefaultKeybindings()
	{
		var gameplayInputActions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_"));

		foreach (string action in gameplayInputActions)
		{
			var inputEvents = InputMap.ActionGetEvents(action);

			if (inputEvents.Count > 0)
			{
				DefaultKeybindings[action] = inputEvents;
			}
		}
	}

	public void SetDefaultValues()
	{
		SetDefaultGeneralValues();
		SetDefaultVideoValues();
		SetDefaultAudioValues();
		SetDefaultControlValues();
	}

	public void SetDefaultGeneralValues()
	{
		Language = DefaultLanguage;
		ActiveController = (InputType)DefaultInputType;
		ControlerPrompts = DefaultControlerPrompts;
	}

	public void SetDefaultVideoValues()
	{
		ScreenMode = DefaultScreenMode;
		ScreenWidth = DisplayServer.ScreenGetSize().X;//DefaultScreenWidth;
		ScreenHeight = DisplayServer.ScreenGetSize().Y;//DefaultScreenHeight;
		ScreenShake = DefaultScreenShake;
		BackgroundBrightness = DefaultBackgroundBrightness;
		PickupOrder = DefaultPickupOrder;
		HelperTransparency = DefaultHelperTransparency;
	}

	public void SetDefaultAudioValues()
	{
		MasterVolume = DefaultMasterVolume;
		MusicVolume = DefaultMusicVolume;
		EffectsVolume = DefaultEffectsVolume;
	}

	public void SetDefaultControlValues()
	{
		SpeedMouse = DefaultMouseSpeed;
		SpeedKeyboard = DefaultKeyboardSpeed;
		SpeedJoypad = DefaultJoypadSpeed;
		ActiveJoypad = string.Empty;
		SetDefaultKeybindings();
	}

	private void SetDefaultKeybindings()
	{
		var gameplayInputActions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_"));

		foreach (string action in DefaultKeybindings.Keys)
		{
			var inputEvents = InputMap.ActionGetEvents(action);

			InputMap.ActionEraseEvents(action);

			foreach (InputEvent inputEvent in DefaultKeybindings[action])
			{
				InputMap.ActionAddEvent(action, inputEvent);
			}
		}

		SaveKeybindings();
	}

	public void LoadSettings()
	{
		_config = new ConfigFile();
		Error error = _config.Load(refs.gameData.ConfigFilePath);

		if (error != Error.Ok)
		{
			SetDefaultValues();
			firstLaunch = true;
		}

		ApplySettings();
	}

	public void SaveSettings()
	{
		// SaveKeybindings();
		_config.Save(refs.gameData.ConfigFilePath);
		ApplySettings();
	}

	public void ChangeKeybinding(string actionToChange, string inputValueToChange, InputEvent newInputValue)
	{
		if (inputValueToChange != string.Empty)
		{
			string oldInputValue = refs.localization.GetInputKey(inputValueToChange);
			var inputEvents = InputMap.ActionGetEvents(actionToChange).Where(a => a.AsText().Contains(oldInputValue));

			InputMap.ActionEraseEvent(actionToChange, inputEvents.ElementAt(0));
		}

		InputMap.ActionAddEvent(actionToChange, newInputValue);
		SaveKeybindings();
	}

	private void SaveKeybindings()
	{
		var gameplayInputActions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_"));

		foreach (string action in gameplayInputActions)
		{
			var inputEvents = InputMap.ActionGetEvents(action);
			_config.SetValue("controls", action, inputEvents);
		}
	}

	private void ApplyKeybindings()
	{
		var gameplayInputActions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_"));

		foreach (string action in gameplayInputActions)
		{
			Godot.Collections.Array<InputEvent> customKeybinds = (Godot.Collections.Array<InputEvent>)_config.GetValue("controls", action);
			InputMap.ActionEraseEvents(action);

			foreach (InputEvent inputEvent in customKeybinds)
			{
				InputMap.ActionAddEvent(action, inputEvent);
			}
		}
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

		ApplyKeybindings();
	}
}