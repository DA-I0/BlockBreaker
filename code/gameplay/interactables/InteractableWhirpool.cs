using Godot;

public partial class InteractableWhirpool : Node2D
{
	[Export] private float _releaseDelay = 1f;
	[Export] private float _cooldown = 10f;

	private bool _isActive = true;

	private Ball _ball = null;

	[Export] private AnimationPlayer _animator;
	[Export] private Timer _timer;

	public override void _Ready()
	{
		AdjustSprite();
	}

	private void OnBodyEntered(Node2D body)
	{
		if (_ball == null && (Ball)body != null)
		{
			CatchBall((Ball)body);
		}
	}

	private void CatchBall(Ball targetBall)
	{
		_ball = targetBall;
		_ball.BallMode = BallMode.frozen;
		_ball.Position = Position;

		float newRotation = Mathf.DegToRad(GD.RandRange(0, 359));
		_ball.Velocity = _ball.Velocity.Rotated(newRotation);

		_timer.Start(_releaseDelay);
		_animator.Pause();
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
