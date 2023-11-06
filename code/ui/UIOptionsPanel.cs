using System.Collections.Generic;
using Godot;

public partial class UIOptionsPanel : UIPanel
{
	[Export] private Label _header;
	[Export] private OptionButton _language;
	[Export] private HSlider _mouseSpeed;
	[Export] private HSlider _keyboardSpeed;
	[Export] private CheckButton _fullscreen;
	[Export] private OptionButton _resolution;
	[Export] private CheckButton _screenShake;
	[Export] private HSlider _backgroundBrightness;
	[Export] private OptionButton _pickupOrder;
	[Export] private HSlider _helperTransparency;
	[Export] private HSlider _masterVolume;
	[Export] private HSlider _musicVolume;
	[Export] private HSlider _effectsVolume;

	private int _activePanel = 0;

	public override void _Ready()
	{
		SetupReferences();
		DisplayActivePanel();
		PopulateLanguageList();
	}

	protected override void Focus()
	{
		if (Visible)
		{
			_focusTarget.GrabFocus();
			ChangeActivePanel(0);
		}
	}

	private void UpdateSettings()
	{
		_language.Selected = FindOptionIndex(_language, TranslationServer.GetLanguageName(refs.settings.Language));
		_mouseSpeed.Value = refs.settings.SpeedMouse;
		_keyboardSpeed.Value = refs.settings.SpeedKeyboard;
		_fullscreen.ButtonPressed = (refs.settings.ScreenMode > 0);
		// _resolution.Selected = FindOptionIndex(_resolution, $"{refs.settings.ScreenWidth}x{refs.settings.ScreenHeight}");
		_screenShake.ButtonPressed = refs.settings.ScreenShake;
		_backgroundBrightness.Value = refs.settings.BackgroundBrightness;
		_pickupOrder.Selected = refs.settings.PickupOrder;
		_helperTransparency.Value = refs.settings.HelperTransparency;
		_masterVolume.Value = refs.settings.MasterVolume;
		_musicVolume.Value = refs.settings.MusicVolume;
		_effectsVolume.Value = refs.settings.EffectsVolume;
	}

	private void SaveSettings()
	{
		refs.settings.Language = TranslationServer.GetLoadedLocales()[_language.Selected];
		refs.settings.SpeedMouse = (float)_mouseSpeed.Value;
		refs.settings.SpeedKeyboard = (float)_keyboardSpeed.Value;
		refs.settings.ScreenMode = _fullscreen.ButtonPressed ? 3 : 0;
		// string resolutionString = _resolution.GetItemText(_resolution.Selected);
		// refs.settings.ScreenWidth = int.Parse(resolutionString.Split("x")[0]);
		// refs.settings.ScreenHeight = int.Parse(resolutionString.Split("x")[1]);
		refs.settings.ScreenShake = _screenShake.ButtonPressed;
		refs.settings.BackgroundBrightness = (float)_backgroundBrightness.Value;
		refs.settings.PickupOrder = _pickupOrder.Selected;
		refs.settings.HelperTransparency = (float)_helperTransparency.Value;
		refs.settings.MasterVolume = (float)_masterVolume.Value;
		refs.settings.MasterVolume = (float)_masterVolume.Value;
		refs.settings.MusicVolume = (float)_musicVolume.Value;
		refs.settings.EffectsVolume = (float)_effectsVolume.Value;

		refs.settings.SaveSettings();
	}

	private void CancelSettings()
	{
		refs.settings.LoadSettings();
		UpdateSettings();
	}

	private void RestoreDefaultSettings()
	{
		switch (_activePanel)
		{
			case 0:
				refs.settings.SetDefaultGeneralValues();
				break;

			case 1:
				refs.settings.SetDefaultVideoValues();
				break;

			case 2:
				refs.settings.SetDefaultAudioValues();
				break;

			case 3:
				refs.settings.SetDefaultControlValues();
				break;

			default:
				refs.settings.SetDefaultValues();
				break;
		}

		UpdateSettings();
	}

	private void ChangeActivePanel(int targetPanelIndex)
	{
		_activePanel = targetPanelIndex;
		DisplayActivePanel();
	}

	private void DisplayActivePanel()
	{
		for (int index = 1; index < GetChildCount() - 2; index++)
		{
			((Control)GetChild(index)).Visible = ((index - 1) == _activePanel);
		}

		UpdateSettings();
		UpdateHeaderText();
	}

	private void PopulateLanguageList()
	{
		List<string> languageNames = new List<string>();

		foreach (string languageCode in TranslationServer.GetLoadedLocales())
		{
			if (!languageNames.Contains(languageCode))
			{
				languageNames.Add(TranslationServer.GetLanguageName(languageCode));
			}
		}

		foreach (string languageName in languageNames)
		{
			_language.AddItem(languageName);
		}
	}

	private void PopulateResolutionList()
	{
		// DisplayServer.get
	}

	private int FindOptionIndex(OptionButton targetList, string optionToFind)
	{
		for (int ind = 0; ind < targetList.ItemCount; ind++)
		{
			if (targetList.GetItemText(ind) == optionToFind)
			{
				return ind;
			}
		}

		return -1;
	}

	private void UpdateHeaderText()
	{
		switch (_activePanel)
		{
			case 0:
				_header.Text = $"{Tr("header_options")} - {Tr("header_general")}";
				break;

			case 1:
				_header.Text = $"{Tr("header_options")} - {Tr("header_video")}";
				break;

			case 2:
				_header.Text = $"{Tr("header_options")} - {Tr("header_audio")}";
				break;

			case 3:
				_header.Text = $"{Tr("header_options")} - {Tr("header_controls")}";
				break;

			default:
				_header.Text = Tr("header_options");
				break;
		}
	}
}
