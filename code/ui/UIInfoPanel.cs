using Godot;

public partial class UIInfoPanel : UIPanel
{
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