using Godot;

public partial class Pickup : Area2D
{
	private const int BaseSpeed = 35;
	private readonly int[] SortingIndex = { 0, 2, 3 };

	[Export] private int _pointValue = 5;
	[Export] private int _customSpeedAdjustment = 0;

	private int _moveSpeed;

	protected SessionController refs;

	public override void _Ready()
	{
		refs = GetNode<SessionController>("/root/GameController");
		ZIndex = SortingIndex[refs.settings.PickupOrder];
		_moveSpeed = (int)(BaseSpeed * refs.SelectedDifficulty.PickupSpeedMultiplier + _customSpeedAdjustment);
	}

	public override void _PhysicsProcess(double delta)
	{
		float newVerticalPosition = Position.Y + (_moveSpeed * (float)delta);
		Position = new Vector2(Position.X, newVerticalPosition);
	}

	protected virtual void OnBodyEntered(Node2D body)
	{
		if ((BasePaddle)body != null)
		{
			ApplyPickup();
			refs.gameScore.ChangeScore(_pointValue, false);
			QueueFree();
		}
	}

	private void OnScreenExited()
	{
		QueueFree();
	}

	protected virtual void ApplyPickup() { }
}
