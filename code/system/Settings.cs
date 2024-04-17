using System.Collections.Generic;
using System.Linq;
using BoGK.Models;
using Godot;

public enum InputType { Joypad, Keyboard, Mouse };

namespace BoGK.GameSystem
{
	public class Settings
	{
		private bool _firstLaunch = false;

		private ConfigFile _defaultConfig;
		private ConfigFile _config;

		private Dictionary<string, Godot.Collections.Array<InputEvent>> DefaultKeybindings = new Dictionary<string, Godot.Collections.Array<InputEvent>>();
		private Dictionary<string, BreakableVariant> _defaultBreakableVariants = new Dictionary<string, BreakableVariant>();
		private Dictionary<string, BreakableVariant> _breakableVariants = new Dictionary<string, BreakableVariant>();
		private InputType _activeInputType = InputType.Keyboard;
		private int _activeControllerId = -1;

		private readonly SessionController refs;

		public event Notification SettingsUpdated;

		public bool FirstLaunch
		{
			get { return _firstLaunch; }
		}

		// Default settings
		public string DefaultLanguage
		{
			get { return (string)_defaultConfig.GetValue("general", "language"); }
		}

		public int DefaultFont
		{
			get { return (int)_defaultConfig.GetValue("general", "font"); }
		}

		public string DefaultControlerPrompts
		{
			get { return (string)_defaultConfig.GetValue("general", "controler_prompts"); }
		}

		public bool DefaultLivesAsText // move to a separate UI category?
		{
			get { return (bool)_defaultConfig.GetValue("general", "lives_as_text"); }
		}

		public bool DefaultStageClearScreen // move to a separate UI category?
		{
			get { return (bool)_defaultConfig.GetValue("general", "stage_clear_screen"); }
		}

		public int DefaultScreenMode
		{
			get { return (int)_defaultConfig.GetValue("display", "screen_mode"); }
		}

		public bool DefaultScreenShake
		{
			get { return (bool)_defaultConfig.GetValue("display", "screen_shake"); }
		}

		public int DefaultPickupOrder
		{
			get { return (int)_defaultConfig.GetValue("display", "pickup_order"); }
		}

		public string DefaultBackgroundColorPalette
		{
			get { return (string)_defaultConfig.GetValue("display", "background_color_palette"); }
		}

		public float DefaultBackgroundBrightness
		{
			get { return (float)_defaultConfig.GetValue("display", "background_brightness"); }
		}

		public float DefaultEffectTransparency
		{
			get { return (float)_defaultConfig.GetValue("display", "effect_transparency"); }
		}

		public string DefaultBreakableColorPalette
		{
			get { return (string)_defaultConfig.GetValue("display", "background_color_palette"); }
		}

		public Dictionary<string, BreakableVariant> DefaultBreakableVariants
		{
			get { return _defaultBreakableVariants; }
		}

		public float DefaultMasterVolume
		{
			get { return (float)_defaultConfig.GetValue("audio", "master_volume"); }
		}

		public float DefaultMusicVolume
		{
			get { return (float)_defaultConfig.GetValue("audio", "music_volume"); }
		}

		public float DefaultEffectsVolume
		{
			get { return (float)_defaultConfig.GetValue("audio", "effects_volume"); }
		}

		public float DefaultMouseSpeed
		{
			get { return (float)_defaultConfig.GetValue("controls", "mouse_speed"); }
		}

		public float DefaultKeyboardSpeed
		{
			get { return (float)_defaultConfig.GetValue("controls", "keyboard_speed"); }
		}

		public float DefaultControllerSpeed
		{
			get { return (float)_defaultConfig.GetValue("controls", "controller_speed"); }
		}

		public bool DefaultControllerVibrations
		{
			get { return (bool)_defaultConfig.GetValue("controls", "controller_vibrations"); }
		}

		public int DefaultInputType
		{
			get { return (int)_defaultConfig.GetValue("controls", "input_type"); }
		}

		// Custom setting
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

		public bool StageClearScreen // move to a separate UI category?
		{
			get { return (bool)_config.GetValue("general", "stage_clear_screen", DefaultStageClearScreen); }
			set { _config.SetValue("general", "stage_clear_screen", value); }
		}

		public int ScreenMode
		{
			get { return (int)_config.GetValue("display", "screen_mode", DefaultScreenMode); }
			set { _config.SetValue("display", "screen_mode", value); }
		}

		public bool ScreenShake
		{
			get { return (bool)_config.GetValue("display", "screen_shake", DefaultScreenShake); }
			set { _config.SetValue("display", "screen_shake", value); }
		}

		public int PickupOrder
		{
			get { return (int)_config.GetValue("display", "pickup_order", DefaultPickupOrder); }
			set { _config.SetValue("display", "pickup_order", value); }
		}

		public string BackgroundColorPalette
		{
			get { return (string)_config.GetValue("display", "background_color_palette", DefaultBackgroundColorPalette); }
			set { _config.SetValue("display", "background_color_palette", value); }
		}

		public float BackgroundBrightness
		{
			get { return (float)_config.GetValue("display", "background_brightness", DefaultBackgroundBrightness); }
			set { _config.SetValue("display", "background_brightness", value); }
		}

		public float EffectTransparency
		{
			get { return (float)_config.GetValue("display", "effect_transparency", DefaultEffectTransparency); }
			set { _config.SetValue("display", "effect_transparency", value); }
		}

		public string BreakableColorPalette
		{
			get { return (string)_config.GetValue("display", "breakable_color_palette", DefaultBreakableColorPalette); }
			set { _config.SetValue("display", "breakable_color_palette", value); }
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
				_config.SetValue("controls", "input_type", (int)value);
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
			LoadDefaultSettings();
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
			Font = DefaultFont;
			ActiveInputType = (InputType)DefaultInputType;
			ControlerPrompts = DefaultControlerPrompts;
			LivesAsText = DefaultLivesAsText;
			StageClearScreen = DefaultStageClearScreen;
		}

		public void SetDefaultVideoValues()
		{
			ScreenMode = DefaultScreenMode;
			ScreenShake = DefaultScreenShake;
			BackgroundBrightness = DefaultBackgroundBrightness;
			PickupOrder = DefaultPickupOrder;
			EffectTransparency = DefaultEffectTransparency;
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

		private void LoadDefaultSettings()
		{
			_defaultConfig = new ConfigFile();
			Error error = _defaultConfig.Load(ProjectSettings.GetSetting("global/DefaultConfigFilePath").ToString());

			if (error != Error.Ok)
			{
				GD.PrintErr("Failed to read default config file.");
			}

			ParseVariantsFromConfig(_defaultConfig, _defaultBreakableVariants);
		}

		public void LoadSettings()
		{
			_config = new ConfigFile();
			Error error = _config.Load(ProjectSettings.GetSetting("global/ConfigFilePath").ToString());

			if (error != Error.Ok)
			{
				SetDefaultValues();
				_firstLaunch = true;
			}

			ApplySettings();
		}

		public void SaveSettings()
		{
			ParseVariantsToConfig();
			_config.Save(ProjectSettings.GetSetting("global/ConfigFilePath").ToString());
			ApplySettings();
			_firstLaunch = false;
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
			ParseVariantsFromConfig(_config, BreakableVariants);

			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), MasterVolume);
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), MusicVolume);
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), EffectsVolume);

			ActiveControllerID = (ActiveController != string.Empty) ? FindConnectedController() : -1;
			ApplyKeybindings();
			SettingsUpdated?.Invoke();
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
			Theme currentTheme = ThemeDB.GetProjectTheme();
			currentTheme.DefaultFont = ResourceLoader.Load<Font>($"res://assets/fonts/{refs.gameData.Fonts[Font].FontName}");
			currentTheme.DefaultFontSize = refs.gameData.Fonts[Font].DefaultSize;
		}

		private void ParseVariantsToConfig()
		{
			HelperMethods.VariantsToConfig(_config, BreakableVariants);
		}

		private void ParseVariantsFromConfig(ConfigFile sourceConfig, Dictionary<string, BreakableVariant> targetVariants)
		{
			string[] variantSections = sourceConfig.GetSections();

			foreach (string section in variantSections)
			{
				if (section.StartsWith("Variant - "))
				{
					BreakableVariant newVariant = HelperMethods.VariantFromConfig(sourceConfig, section);
					targetVariants[section.Split(" - ")[^1]] = newVariant;
				}
			}
		}

		public BreakableVariant FindVariant(string brekableName)
		{
			try
			{
				return BreakableVariants[brekableName];
			}
			catch { }

			try
			{
				return DefaultBreakableVariants[brekableName];
			}
			catch { }

			return new BreakableVariant(string.Empty, 0, new Color(1, 1, 1, 1));
		}
	}
}