using Godot;

public partial class Pickup : Area2D
{
	[Export] private int _pointValue = 5;
	[Export] private float _speed = 10f;

	protected SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;
		ZIndex = refs.settings.PickupOrder;
	}

	public override void _PhysicsProcess(double delta)
	{
		float newVerticalPosition = Position.Y + (_speed * (float)delta);
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
