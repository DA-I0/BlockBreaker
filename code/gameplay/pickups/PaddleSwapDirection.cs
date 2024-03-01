public partial class PaddleSwapDirection : Pickup
{
	protected override void ApplyPickup()
	{
		refs.paddle.MovementDirection *= -1;
	}
}
