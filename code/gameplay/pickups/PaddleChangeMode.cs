using Godot;

public partial class PaddleChangeMode : Pickup
{
	[Export] PaddleMode _paddleMode;

	protected override void ApplyPickup()
	{
		refs.paddle.SetPaddleMode(_paddleMode);
	}
}
