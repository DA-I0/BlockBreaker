public partial class PickupPaddleDirection : Pickup
{
	protected override void ApplyPickup()
	{
		refs.paddle.MovementDirection *= -1;
	}
}
