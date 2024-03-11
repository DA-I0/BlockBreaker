using Godot;

public partial class BallChangeSpeed : Pickup
{
	[Export] private float _speedChange = 2f;

	protected override void ApplyPickup()
	{
		foreach (Ball ball in refs.Balls)
		{
			ball.SpeedMultiplier += _speedChange;
		}
	}
}
