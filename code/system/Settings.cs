using System.Collections.Generic;
using System.Linq;
using BoGK.Models;
using Godot;

public enum InputType { Joypad, Keyboard, Mouse };

public class Settings
{
	public bool firstLaunch = false;

	private const string DefaultLanguage = "en";
	private const int DefaultFont = 0;
	private const string DefaultControlerPrompts = "generic";
	private const int DefaultInputType = 1;
	private const bool DefaultLivesAsText = false;
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
	private const float DefaultControllerSpeed = 1f;
	private const bool DefaultControllerVibrations = true;
	private Dictionary<string, Godot.Collections.Array<InputEvent>> DefaultKeybindings = new Dictionary<string, Godot.Collections.Array<InputEvent>>();
	private Dictionary<string, BreakableVariant> DefaultBreakableVariants = new Dictionary<string, BreakableVariant>{
		{"barrier_sturdy", new BreakableVariant("barrier_sturdy", 0, new Color(1,1,1,1))},
		{"block_basic", new BreakableVariant("block_basic", 0, new Color(1,1,1,1))},
		{"block_sturdy", new BreakableVariant("block_sturdy", 0, new Color(1,1,1,1))},
		{"coffin", new BreakableVariant("coffin", 0, new Color(1,1,1,1))},
		{"explosives", new BreakableVariant("explosives", 0, new Color(1,1,1,1))},
		{"safety_net", new BreakableVariant("safety_net", 0, new Color(1,1,1,1))}
	};

	private Dictionary<string, BreakableVariant> _breakableVariants = new Dictionary<string, BreakableVariant>();
	private InputType _activeInputType = InputType.Keyboard;
	private int _activeControllerId = -1;
	private ConfigFile _config;
	private SessionController refs;

	public string Language
	{
		get { return (string)_config.GetValue("general", "language", DefaultLanguage); }
		set { _config.SetValue("general", "language", value); }
	}

	public int Font
	{
		get { return (int)_config.GetValue("general", "font", DefaultFont); }
		set { _config.SetValue("general", "font", value); }
	}

	public string ControlerPrompts
	{
		get { return (string)_config.GetValue("general", "controler_prompts", DefaultControlerPrompts); }
		set { _config.SetValue("general", "controler_prompts", value); }
	}

	public bool LivesAsText // move to a separate UI category?
	{
		get { return (bool)_config.GetValue("general", "lives_as_text", DefaultLivesAsText); }
		set { _config.SetValue("general", "lives_as_text", value); }
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

	public Dictionary<string, BreakableVariant> BreakableVariants
	{
		get { return _breakableVariants; }
		set { _breakableVariants = value; }
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

	public float SpeedController
	{
		get { return (float)_config.GetValue("controls", "controller_speed", DefaultControllerSpeed); }
		set { _config.SetValue("controls", "controller_speed", value); }
	}

	public bool ControllerVibrations
	{
		get { return (bool)_config.GetValue("controls", "controller_vibrations", DefaultControllerVibrations); }
		set { _config.SetValue("controls", "controller_vibrations", value); }
	}

	public InputType ActiveInputType
	{
		get { return _activeInputType; }
		set
		{
			_activeInputType = value;
			_config.SetValue("general", "inputType", (int)value);
		}
	}

	public string ActiveController
	{
		get { return (string)_config.GetValue("controls", "active_controller", string.Empty); }
		set { _config.SetValue("controls", "active_controller", value); }
	}

	public int ActiveControllerID
	{
		get { return _activeControllerId; }
		set { _activeControllerId = value; }
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
		ActiveInputType = (InputType)DefaultInputType;
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
		BreakableVariants = DefaultBreakableVariants;
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
		SpeedController = DefaultControllerSpeed;
		ActiveController = string.Empty;
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
		Error error = _config.Load(ProjectSettings.GetSetting("global/ConfigFilePath").ToString());

		if (error != Error.Ok)
		{
			SetDefaultValues();
			firstLaunch = true;
		}

		ApplySettings();
	}

	public void SaveSettings()
	{
		ParseVariantsToConfig();
		_config.Save(ProjectSettings.GetSetting("global/ConfigFilePath").ToString());
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
		ApplyFont();

		DisplayServer.WindowSetMode((DisplayServer.WindowMode)ScreenMode, 0);
		Vector2I newResolution = new Vector2I(ScreenWidth, ScreenHeight);
		DisplayServer.WindowSetSize(newResolution, 0);
		ParseVariantsFromConfig();

		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), MasterVolume);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), MusicVolume);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), EffectsVolume);

		ActiveControllerID = (ActiveController != string.Empty) ? FindConnectedController() : -1;
		ApplyKeybindings();
	}

	private int FindConnectedController()
	{
		foreach (int index in Input.GetConnectedJoypads())
		{
			if (Input.GetJoyName(index) == ActiveController)
			{
				return index;
			}
		}

		return -1;
	}

	private void ApplyFont()
	{
		ProjectSettings.SetSetting("gui/theme/custom_font", $"res://assets/fonts/{refs.gameData.Fonts[Font].FontName}"); // needed?

		Theme currentTheme = ThemeDB.GetDefaultTheme();
		currentTheme.DefaultFont = ResourceLoader.Load<Font>(ProjectSettings.GetSetting("gui/theme/custom_font").ToString());
		currentTheme.DefaultFontSize = refs.gameData.Fonts[Font].DefaultSize;
		GD.Print("setting default font: " + currentTheme.DefaultFont + " of size: " + currentTheme.DefaultFontSize);
	}

	private void ParseVariantsToConfig()
	{
		HelperMethods.VariantsToConfig(_config, BreakableVariants);
	}

	private void ParseVariantsFromConfig()
	{
		string[] variantSections = _config.GetSections();

		foreach (string section in variantSections)
		{
			if (section.StartsWith("Variant - "))
			{
				BreakableVariant newVariant = HelperMethods.VariantFromConfig(_config, section);
				BreakableVariants[section.Split(" - ")[^1]] = newVariant;
			}
		}
	}

	public BreakableVariant FindVariant(string brekableName)
	{
		GD.Print("looking for: " + brekableName);
		try
		{
			return BreakableVariants[brekableName];
		}
		catch { }

		GD.Print("> couldn't find: " + brekableName);

		return new BreakableVariant(string.Empty, 0, new Color(1, 1, 1, 1));
	}
}