using Godot;

public partial class UIPatchNotesPanel : UIPanel
{
	[Export] private RichTextLabel _content;

	public override void _Ready()
	{
		SetupBaseReferences();
		LoadPatchNotes();
	}

	public void LoadPatchNotes()
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
