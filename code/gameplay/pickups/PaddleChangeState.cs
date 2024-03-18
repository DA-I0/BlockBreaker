using Godot;

namespace BoGK.Gameplay
{
	public partial class PaddleChangeState : Pickup
	{
		[Export] private PaddleState _state;
		[Export] private float _time = 2f;

		protected override void ApplyPickup()
		{
			refs.paddle.SetPaddleState(_state, _time);
		}
	}
}