using System.Collections.Generic;
using Godot;

public partial class UIFirstLanguageSelection : Control
{
	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;
		PopulateLanguageList();
	}

	private void PopulateLanguageList()
	{
		GridContainer gridContainer = GetChild(1) as GridContainer;

		List<string> languageNames = new List<string>();

		foreach (string languageCode in TranslationServer.GetLoadedLocales())
		{
			if (!languageNames.Contains(languageCode))
			{
				languageNames.Add(TranslationServer.GetLanguageName(languageCode));
			}
		}

		for (int index = 0; index < languageNames.Count; index++)
		{
			Button nextLanguage = new Button();
			nextLanguage.Text = languageNames[index];

			int languageIndex = index;
			nextLanguage.Pressed += () => SetInitialLanguage(languageIndex);

			nextLanguage.SizeFlagsHorizontal = SizeFlags.ShrinkCenter | SizeFlags.Expand;
			nextLanguage.SizeFlagsVertical = SizeFlags.ShrinkCenter;

			GetChild(1).AddChild(nextLanguage);
		}
	}

	private void SetInitialLanguage(int index)
	{
		refs.settings.Language = TranslationServer.GetLoadedLocales()[index];
		refs.settings.SaveSettings();
		(GetParent() as Control).Visible = false;
	}
}