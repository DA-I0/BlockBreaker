using Godot;

namespace BoGK.Gameplay
{
	public partial class TeleportDoor : Interactable
	{
		[Export] private TeleportDoor _linkedDoor;
		[Export] private float _ballDisplacement = 5f;


		protected override void UpdateState()
		{
			string state = (_isActived && _linkedDoor != null) ? "idle_enabled" : "idle_disabled";
			_animator.Play(state);
		}

		private void TeleportBall(Ball ball)
		{
			Toggle();

			Vector2 newBallPosition = Vector2.Down * _ballDisplacement;
			ball.Position = Position + newBallPosition.Rotated(Rotation);

			float newBallRotation = (Rotation - _linkedDoor.Rotation) + Mathf.DegToRad(180); // need improvements for (-)90 degree changes
			ball.Velocity = ball.Velocity.Rotated(newBallRotation);
			ball.ResetSpeedTrail();

			_timer.Start(_cooldown);
		}

		private void OnBodyEntered(Node2D body)
		{
			if ((Ball)body != null && _linkedDoor != null)
			{
				_linkedDoor.TeleportBall((Ball)body);
				Toggle();
				_timer.Start(_cooldown);
			}
		}
	}
}