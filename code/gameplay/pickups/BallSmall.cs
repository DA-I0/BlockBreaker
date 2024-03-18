using Godot;

namespace BoGK.Gameplay
{
	public partial class BallSmall : Pickup
	{
		[Export] private float _newSize = 0.5f;

		protected override void ApplyPickup()
		{
			foreach (Ball ball in refs.Balls)
			{
				ball.ChangeSize(_newSize);
			}
		}
	}
}