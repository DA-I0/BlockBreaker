using Godot;

namespace BoGK.Gameplay
{
	public partial class BallMulti : Pickup
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
}