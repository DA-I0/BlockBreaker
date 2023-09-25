public partial class PickupExtraLife : Pickup
{
	protected override void ApplyPickup()
	{
		refs.health.ChangeLives(1);
	}
}
