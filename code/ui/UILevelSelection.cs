using Godot;

public partial class UILevelSelection : Panel
{
	[Export] private PackedScene _buttonPrefab;

	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		((UIController)GetNode("../..")).RefreshUI += Focus;
		PopulateLevels();
	}

	private void PopulateLevels()
	{
		for (int index = 0; index < refs.gameData.Levels.Count; index++)
		{
			UILevelButton newButton = (UILevelButton)_buttonPrefab.Instantiate();
			newButton.ButtonSetup(index, refs.gameData.Levels[index]);
			GetChild(0).AddChild(newButton);
		}
	}

	private void Focus()
	{
		if (Visible)
		{
			((Button)GetChild(0).GetChild(0)).GrabFocus();
		}
	}
}