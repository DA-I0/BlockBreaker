using Godot;

public class EmergencyNet : Skill
{
	private const string PrefabPath = "res://prefabs/skills/emergency_net.tscn";

	public EmergencyNet()
	{
		_activationPointsCost = 20;
	}

	public override void Activate()
	{
		if (_activationPoints >= _activationPointsCost)
		{
			Node safetyNet = refs.gameElements.GetNodeOrNull("SafetyNet");

			if (safetyNet != null)
			{
				refs.gameElements.RemoveChild(safetyNet);
				safetyNet.QueueFree();
			}

			safetyNet = ResourceLoader.Load<PackedScene>(PrefabPath).Instantiate();
			refs.gameElements.CallDeferred("add_child", safetyNet);
			OnActivation();
		}
	}
}