using Godot;

namespace BoGK.Gameplay
{
	public partial class SafetyNet : Pickup
	{
		protected override void ApplyPickup()
		{
			Node safetyNet = refs.gameElements.GetNodeOrNull("SafetyNet");

			if (safetyNet != null)
			{
				refs.gameElements.RemoveChild(safetyNet);
				((ObjectCleaner)safetyNet).Destroy();
			}

			safetyNet = ResourceLoader.Load<PackedScene>("res://prefabs/safety_net.tscn").Instantiate();
			refs.gameElements.CallDeferred(Node.MethodName.AddChild, safetyNet);
		}
	}
}