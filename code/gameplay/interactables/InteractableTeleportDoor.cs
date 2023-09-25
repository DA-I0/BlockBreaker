using Godot;

public partial class InteractableTeleportDoor : Node2D
{
	[Export] private bool _isActive;
	[Export] private InteractableTeleportDoor _linkedDoor;
	[Export] private float _ballDisplacement = 0.6f;
	[Export] private float _cooldown = 3f;

	[Export] private AnimationPlayer _animator;
	[Export] private Timer _timer;

	public override void _Ready()
	{
		AdjustSprite();
	}

	private void OnBodyEntered(Node2D body)
	{
		if ((Ball)body != null)
		{
			_linkedDoor.TeleportBall((Ball)body);
			Toggle();
			_timer.Start(_cooldown);
		}
	}

	private void TeleportBall(Ball ball)
	{
		Vector2 newBallPosition = Vector2.Down * _ballDisplacement;
		ball.Position = Position + newBallPosition.Rotated(Rotation);

		float newBallRotation = (Rotation - _linkedDoor.Rotation) + Mathf.DegToRad(180);
		ball.Velocity = ball.Velocity.Rotated(newBallRotation);

		Toggle();
		_timer.Start(_cooldown);
	}

	private void AdjustSprite()
	{
		string state = _isActive ? "idle_enabled" : "idle_disabled";
		_animator.Play(state);
	}

	public void Toggle()
	{
		_isActive = !_isActive;
		AdjustSprite();
	}
}

