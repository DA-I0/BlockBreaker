using Godot;

public partial class UIChangelogPanel : UIPanel
{
	[Export] private RichTextLabel _content;

	public override void _Ready()
	{
		SetupReferences();
		LoadChangelog();
	}

	public void LoadChangelog()
	{
		_content.Text = refs.gameData.PatchNotes;
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible)
		{
			if (@event.IsActionPressed("ui_down"))
			{
				((ScrollContainer)_focusTarget).ScrollVertical += 10;
			}

			if (@event.IsActionPressed("ui_up"))
			{
				((ScrollContainer)_focusTarget).ScrollVertical -= 10;
			}
		}
	}
}
