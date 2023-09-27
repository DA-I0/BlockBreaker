using Godot;

public partial class PickupPaddleResize : Pickup
{
	[Export] private int _sizeChange = 1;

	protected override void ApplyPickup()
	{
		refs.paddle.ChangeSize(_sizeChange);
	}
}