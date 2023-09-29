using Godot;

public partial class UIInfo : Panel
{
	[Export] ScrollContainer _focusTarget;

	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		((UIController)GetNode("../..")).RefreshUI += Focus;
	}

	private void Focus()
	{
		if (Visible)
		{
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