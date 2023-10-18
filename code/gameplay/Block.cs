using Godot;

public partial class Block : Breakable
{
	[Export] private double _pickupSpawnChance = 0.1f;
	[Export] private PackedScene[] _pickups;

	public override void Damage(int value)
	{
		_health -= value;

		if (_health <= 0)
		{
			SpawnPickup();
			Destroy();
		}

		_timer?.Start(_shakeDuration);
		AdjustSprite();
	}

	private void SpawnPickup()
	{
		if (_pickups.Length < 1)
		{
			return;
		}

		double dropRandomization = GD.RandRange(0.0, 1.0);

		if (dropRandomization <= _pickupSpawnChance)
		{
			int pickupType = GD.RandRange(0, _pickups.Length - 1);
			Area2D pickup = _pickups[pickupType].Instantiate() as Area2D;
			GetNode("../../..").CallDeferred("add_child", pickup);
			pickup.Position = Position;
		}
	}
}
