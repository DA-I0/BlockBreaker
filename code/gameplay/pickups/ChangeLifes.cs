using Godot;

namespace BoGK.Gameplay
{
	public partial class ChangeLifes : Pickup
	{
		[Export] private int _amount = 1;

		protected override void ApplyPickup()
		{
			refs.health.ChangeLives(_amount);
		}
	}
}