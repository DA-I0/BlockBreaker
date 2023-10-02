using Godot;

public partial class PickupPaddleMode : Pickup
{
	[Export] PaddleMode _paddleMode;

	protected override void ApplyPickup()
	{
		refs.paddle.SetPaddleMode(_paddleMode);
	}
}
