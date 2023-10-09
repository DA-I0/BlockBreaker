using Godot;

public partial class UILevelPanel : UIPanel
{
	[Export] private PackedScene _buttonPrefab;

	public override void _Ready()
	{
		SetupReferences();
		PopulateLevels();
	}

	private void PopulateLevels()
	{
		for (int index = 0; index < refs.gameData.Levels.Count; index++)
		{
			UILevelButton newButton = (UILevelButton)_buttonPrefab.Instantiate();
			newButton.ButtonSetup(index, refs.gameData.Levels[index]);
			GetChild(1).AddChild(newButton);
		}
	}

	protected override void Focus()
	{
		if (Visible)
		{
			((Button)(GetChild(1).GetChild(0))).GrabFocus();
		}
	}
}