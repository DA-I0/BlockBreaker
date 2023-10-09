using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class UIOptionsPanel : UIPanel
{
	[Export] private OptionButton _language;
	[Export] private HSlider _mouseSpeed;
	[Export] private HSlider _keyboardSpeed;
	[Export] private CheckButton _fullscreen;
	[Export] private OptionButton _resolution;
	[Export] private CheckButton _screenShake;
	[Export] private HSlider _masterVolume;
	[Export] private HSlider _musicVolume;
	[Export] private HSlider _effectsVolume;

	public override void _Ready()
	{
		SetupReferences();
		UpdateSettings();
		PopulateLanguageList();
	}

	private void UpdateSettings()
	{
		_language.Selected = FindOptionIndex(_language, TranslationServer.GetLanguageName(refs.settings.Language));
		_mouseSpeed.Value = refs.settings.SpeedMouse;
		_keyboardSpeed.Value = refs.settings.SpeedKeyboard;
		_fullscreen.ButtonPressed = (refs.settings.ScreenMode > 0);
		_resolution.Selected = FindOptionIndex(_resolution, $"{refs.settings.ScreenWidth}x{refs.settings.ScreenHeight}");
		_screenShake.ButtonPressed = refs.settings.ScreenShake;
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
		string resolutionString = _resolution.GetItemText(_resolution.Selected);
		refs.settings.ScreenWidth = int.Parse(resolutionString.Split("x")[0]);
		refs.settings.ScreenHeight = int.Parse(resolutionString.Split("x")[1]);
		refs.settings.ScreenShake = _screenShake.ButtonPressed;
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
		refs.settings.SetDefaultValues();
		UpdateSettings();
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

		languageNames.Sort();

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
}
