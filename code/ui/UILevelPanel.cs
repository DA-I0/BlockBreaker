using Godot;

public partial class UILevelPanel : UIPanel
{
	public override void _Ready()
	{
		SetupReferences();
		PopulateLevels();
	}

	private void PopulateLevels()
	{
		for (int index = 0; index < refs.gameData.Levels.Count; index++)
		{
			Button newButton = new Button();
			newButton.Text = refs.gameData.Levels[index].Replace(".tscn", "");

			int levelIndex = index;
			newButton.Pressed += () => refs.SelectLevel(levelIndex);

			newButton.SizeFlagsHorizontal = SizeFlags.ShrinkCenter | SizeFlags.Expand;
			newButton.SizeFlagsVertical = SizeFlags.ShrinkCenter;

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