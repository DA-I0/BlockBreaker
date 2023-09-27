using Godot;

public partial class PickupBallFast : Pickup
{
	[Export] private float _newSpeedMultiplier = 2f;

	protected override void ApplyPickup()
	{
		foreach (Ball ball in refs.Balls)
		{
			ball.ChangeSpeedMultiplier(_newSpeedMultiplier);
		}
	}
}