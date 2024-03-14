using System.Collections.Generic;
using Godot;

namespace BoGK.UI
{
	public partial class UIOptionsGeneralPanel : UIOptionsBaseCategory
	{
		[Export] private OptionButton _language;
		[Export] private OptionButton _font;
		[Export] private OptionButton _controllerType;
		[Export] private CheckButton _livesDisplay;
		[Export] private CheckButton _stageClearDisplay;

		public override void _Ready()
		{
			SetupBaseReferences();
			_mainPanel.FinalizeSetup += PopulateLanguageList;
		}

		protected override void ToggleFocus()
		{
			_focusIndex = (_focusIndex < _focusTarget.Length - 1) ? _focusIndex + 1 : 0;
			_focusTarget[_focusIndex].GrabFocus();
		}

		public override void UpdateSettings()
		{
			_language.Selected = HelperMethods.FindOptionIndex(_language, HelperMethods.GetLocalizedLanguage(Refs.settings.Language));
			_font.Selected = Refs.settings.Font;
			_controllerType.Selected = HelperMethods.FindOptionIndex(_controllerType, Refs.settings.ControlerPrompts);
			_livesDisplay.ButtonPressed = Refs.settings.LivesAsText;
			_stageClearDisplay.ButtonPressed = Refs.settings.StageClearScreen;
		}

		public override void ApplySettings()
		{
			Refs.settings.Language = TranslationServer.GetLoadedLocales()[_language.Selected];
			Refs.settings.Font = _font.Selected;
			Refs.settings.ControlerPrompts = _controllerType.GetItemText(_controllerType.Selected).ToLower();
			Refs.settings.LivesAsText = _livesDisplay.ButtonPressed;
			Refs.settings.StageClearScreen = _stageClearDisplay.ButtonPressed;
		}

		private void CancelSettings()
		{
			Refs.settings.LoadSettings();
			UpdateSettings();
		}

		public override void RestoreDefaultSettings()
		{
			Refs.settings.SetDefaultGeneralValues();
		}

		private void PopulateLanguageList()
		{
			List<string> languageNames = new List<string>();

			foreach (string languageCode in TranslationServer.GetLoadedLocales())
			{
				if (!languageNames.Contains(languageCode))
				{
					languageNames.Add(HelperMethods.GetLocalizedLanguage(languageCode));
				}
			}

			foreach (string languageName in languageNames)
			{
				_language.AddItem(languageName);
			}
		}
	}
}