using Godot;

namespace BoGK.Gameplay
{
	public partial class PaddleChangeMode : Pickup
	{
		[Export] private PaddleMode _paddleMode;

		protected override void ApplyPickup()
		{
			refs.paddle.SetPaddleMode(_paddleMode);
		}
	}
}