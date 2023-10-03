using Godot;

public partial class PickupPaddleFreeze : Pickup
{
	[Export] private float _time = 2f;

	protected override void ApplyPickup()
	{
		refs.paddle.SetPaddleState(PaddleState.frozen, _time);
	}
}
