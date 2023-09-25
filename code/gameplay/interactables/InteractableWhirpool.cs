using Godot;

public partial class InteractableWhirpool : Node2D
{
	[Export] private float _releaseDelay = 1f;
	[Export] private float _cooldown = 10f;

	private bool _isActive = true;

	private Ball ball = null;

	[Export] private AnimationPlayer _animator;
	[Export] private Timer _timer;

	public override void _Ready()
	{
		AdjustSprite();
	}

	private void OnBodyEntered(Node2D body)
	{
		if (ball == null && (Ball)body != null)
		{
			CatchBall((Ball)body);
		}
	}

	private void CatchBall(Ball targetBall)
	{
		ball = targetBall;
		ball.BallMode = BallMode.frozen;
		ball.Position = Position;
		_timer.Start(_releaseDelay);
	}

	private void ReleaseBall()
	{
		float newRotation = Mathf.DegToRad(GD.RandRange(0, 359));
		ball.Velocity = ball.Velocity.Rotated(newRotation);
		ball.BallMode = BallMode.moving;
		ball = null;
	}

	private void AdjustSprite()
	{
		string state = _isActive ? "idle_enabled" : "idle_disabled";
		_animator.Play(state);
	}

	private void Toggle()
	{
		_isActive = !_isActive;

		if (!_isActive)
		{
			ReleaseBall();
			_timer.Start(_cooldown);
		}

		AdjustSprite();
	}
}
