public partial class PickupGhost : Pickup
{
	protected override void ApplyPickup()
	{
		refs.health.ChangeLives(-1);
	}
}
