using Godot;

public partial class PickupBallMulti : Pickup
{
	[Export] private float _angleChange = 15f;

	protected override void ApplyPickup()
	{
		int ballCount = refs.Balls.Count;
		for (int i = 0; i < ballCount; i++)
		{
			Ball sourceBall = (Ball)refs.Balls[i];
			sourceBall.SpawnCopy(_angleChange);
			sourceBall.SpawnCopy(-_angleChange);
		}
	}
}
