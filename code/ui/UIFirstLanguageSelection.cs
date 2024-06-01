using System.Collections.Generic;
using Godot;

namespace BoGK.UI
{
	public partial class UIFirstLanguageSelection : Control
	{
		[Export] private GridContainer _languageContainer;

		private int _defaultLanguageIndex = 0;

		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			PopulateLanguageList();
		}

		public void Focus()
		{
			if (!refs.settings.FirstLaunch)
			{
				Disable();
			}

			_languageContainer.GetChild<Button>(_defaultLanguageIndex).GrabFocus();
		}

		private void PopulateLanguageList()
		{
			List<string> languageNames = new List<string>();

			foreach (string languageCode in TranslationServer.GetLoadedLocales())
			{
				if (!languageNames.Contains(languageCode))
				{
					languageNames.Add(GameSystem.HelperMethods.GetLocalizedLanguage(languageCode));
				}
			}

			for (int index = 0; index < languageNames.Count; index++)
			{
				Button nextLanguage = new Button();

				if (CheckIfDefaultLanguage(languageNames[index]))
				{
					nextLanguage.Text = languageNames[index];
					_defaultLanguageIndex = index;
				}
				else
				{
					nextLanguage.Text = $"{languageNames[index]}*";

				}

				int languageIndex = index;

				nextLanguage.Pressed += () => SetInitialLanguage(languageIndex);

				nextLanguage.SizeFlagsHorizontal = SizeFlags.ShrinkCenter | SizeFlags.Expand;
				nextLanguage.SizeFlagsVertical = SizeFlags.ShrinkCenter;

				_languageContainer.AddChild(nextLanguage);
			}
		}

		private void SetInitialLanguage(int index)
		{
			refs.settings.Language = TranslationServer.GetLoadedLocales()[index];
			refs.settings.SaveSettings();
			Disable();
		}

		private bool CheckIfDefaultLanguage(string language)
		{
			return language == TranslationServer.GetLanguageName(refs.settings.DefaultLanguage);
		}

		private void Disable()
		{
			GetParent<Control>().Visible = false;
			GetParent().GetParent<UIControler>().FocusOnButtons();
		}
	}
}