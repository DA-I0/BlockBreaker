using Godot;

namespace BoGK.Gameplay
{
	public partial class Whirlpool : Interactable
	{
		[Export] private float _releaseDelay = 1f;

		private Ball _ball = null;

		protected override void Toggle()
		{
			_isActived = !_isActived;

			if (!_isActived)
			{
				ReleaseBall();
				_timer.Start(_cooldown);
			}

			UpdateState();
		}

		private void CatchBall(Ball targetBall)
		{
			_ball = targetBall;
			_ball.BallMode = BallMode.spinning;
			_ball.Position = Position;

			float newRotation = Mathf.DegToRad(GD.RandRange(0, 359));
			_ball.Velocity = _ball.Velocity.Rotated(newRotation);

			_timer.Start(_releaseDelay);
		}

		private void ReleaseBall()
		{
			if (_ball == null)
			{
				return;
			}

			_ball.BallMode = BallMode.moving;
			_ball = null;
		}

		private void OnBodyEntered(Node2D body)
		{
			if (_ball == null && (Ball)body != null)
			{
				CatchBall((Ball)body);
			}
		}
	}
}