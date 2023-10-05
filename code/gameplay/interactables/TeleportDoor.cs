using Godot;

public partial class TeleportDoor : Interactable
{
	[Export] private TeleportDoor _linkedDoor;
	[Export] private float _ballDisplacement = 0.6f;

	private void TeleportBall(Ball ball)
	{
		Vector2 newBallPosition = Vector2.Down * _ballDisplacement;
		ball.Position = Position + newBallPosition.Rotated(Rotation);

		float newBallRotation = (Rotation - _linkedDoor.Rotation) + Mathf.DegToRad(180);
		ball.Velocity = ball.Velocity.Rotated(newBallRotation);

		Toggle();
		_timer.Start(_cooldown);
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
}