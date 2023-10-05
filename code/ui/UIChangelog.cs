using Godot;

public partial class UIChangelog : Panel
{
	[Export] private ScrollContainer _focusTarget;
	[Export] private RichTextLabel _content;
	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		((UIController)GetNode("../..")).RefreshUI += LoadChangelog;
	}

	public void LoadChangelog()
	{
		if (Visible)
		{
			_content.Text = refs.gameData.Changelog;
			_focusTarget.GrabFocus();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible)
		{
			if (@event.IsActionPressed("ui_down"))
			{
				_focusTarget.ScrollVertical += 10;
			}

			if (@event.IsActionPressed("ui_up"))
			{
				_focusTarget.ScrollVertical -= 10;
			}
		}
	}
}
