using Godot;

public partial class PickupSafetyNet : Pickup
{
	protected override void ApplyPickup()
	{
		Node safetyNet = refs.gameElements.GetNodeOrNull("SafetyNet");

		if (safetyNet != null)
		{
			refs.gameElements.RemoveChild(safetyNet);
			safetyNet.QueueFree();
		}

		safetyNet = ResourceLoader.Load<PackedScene>("res://prefabs/safety_net.tscn").Instantiate();
		refs.gameElements.CallDeferred("add_child", safetyNet);
	}
}
