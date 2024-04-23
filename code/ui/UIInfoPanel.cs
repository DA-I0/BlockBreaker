using Godot;

namespace BoGK.UI
{
	public partial class UIInfoPanel : UIPanel
	{
		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("ui_down"))
			{
				((ScrollContainer)_focusTarget[0]).ScrollVertical += 10;
			}

			if (@event.IsActionPressed("ui_up"))
			{
				((ScrollContainer)_focusTarget[0]).ScrollVertical -= 10;
			}
		}
	}
}