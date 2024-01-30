using Godot;

public partial class SpeedPad : Interactable
{
	[Export] private float _boost = 1f;

	private void OnBodyEntered(Node2D body)
	{
		if ((Ball)body != null)
		{
			((Ball)body).ChangeTempSpeedMultiplier(_boost);
			Toggle();
			_timer.Start(_cooldown);
		}
	}
}
