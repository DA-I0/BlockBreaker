using Godot;

public enum BallMode { idle, angleSelection, moving, frozen };

public partial class Ball : CharacterBody2D
{
	[Export] private int _maxReleaseAngle = 60;
	[Export] private float _baseSpeed = 10;
	[Export] private float _boostDrag = 0.01f;
	[Export] private float _idlePositionOffset = 7f;

	private BallMode _ballMode = BallMode.idle;

	private float _difficultySpeedMultiplier = 1f;
	private float _difficultyArrowSpeed = 1f;
	private float _speedMultiplier = 1f;

	[Export] private float _multiBallSpawnAngle = 30f;

	private bool _increasingRotation = true;
	private float _startingRotation = 0;

	private float _speed;

	[Export] private Sprite2D _sprite;
	[Export] private Sprite2D _arrow;
	[Export] private Timer _arrowTimer;
	[Export] private CollisionShape2D _collider;
	[Export] private AnimationPlayer _animator;

	private SessionController refs;

	public BallMode BallMode
	{
		get { return _ballMode; }
		set { _ballMode = value; }
	}

	public float SpeedMultiplier
	{
		get { return _speedMultiplier; }
	}

	public override void _Ready()
	{
		SetupReferences();
		SetInitialValues();
		_animator.AnimationSetNext("bounce", "roll");
	}

	public override void _PhysicsProcess(double delta)
	{
		_arrow.Visible = (_ballMode == BallMode.angleSelection);

		switch (_ballMode)
		{
			case BallMode.frozen:
				_animator.Play("idle");
				break;

			case BallMode.moving:
				Move(delta);
				break;

			default:
				Position = (refs.paddle != null) ? new Vector2(refs.paddle.Position.X, refs.paddle.Position.Y - _idlePositionOffset) : Vector2.Zero;
				_animator.Play("idle");
				break;

		}
	}

	public override void _Input(InputEvent @event)
	{

		if (@event.IsActionReleased("game_play"))
		{
			switch (_ballMode)
			{
				case BallMode.angleSelection:
					PlayBall();
					break;

				case BallMode.moving:
					// Shake();
					break;

				default:
					refs.paddle.blockMovement = true;
					_ballMode = BallMode.angleSelection;
					_arrowTimer.Start(0.001f);
					break;
			}
		}
	}

	public void SpawnCopy(float angleChange)
	{
		var newBall = (Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate();
		GetParent().CallDeferred(Node.MethodName.AddChild, newBall);
		newBall.CallDeferred("SetupCloneBall", Position, angleChange, _collider.Scale.X, _speedMultiplier);
	}

	// NOTE: Not sure about implementation of this one but it works for now.
	private void SetupCloneBall(Vector2 position, float angleChange, float size, float speedMultiplier)
	{
		Position = position;
		_arrow.Rotation += Rotation + Mathf.DegToRad(angleChange);
		ChangeSize(size);
		ChangeSpeedMultiplier(speedMultiplier);
		PlayBall();
	}

	private void PlayBall()
	{
		if (refs != null)
		{
			refs.paddle.blockMovement = false;
		}
		UpdateSpeed();
		Velocity = new Vector2(0, -_speed).Rotated(_arrow.Rotation);
		_arrowTimer.Stop();
		_ballMode = BallMode.moving;
	}

	public void AddVelocity(Vector2 velocity)
	{
		// Velocity += velocity * 0.1f;
	}

	public void ChangeSpeedMultiplier(float value)
	{
		_speedMultiplier = value;
		UpdateSpeed();
	}

	public void ChangeSize(float value)
	{
		_sprite.Scale = new Vector2(value, value);
		_collider.Scale = new Vector2(value, value);
	}

	public void Reset()
	{
		RotationDegrees = 0;
		_sprite.RotationDegrees = 0;
		_arrow.RotationDegrees = 0;
		ChangeSpeedMultiplier(1f);
		ChangeSize(1f);
		_ballMode = BallMode.idle;
		_animator.Play("roll", 0);
	}

	public float GetBallSpeed()
	{
		return _speedMultiplier;
	}

	public float GetBallSize()
	{
		return _collider.Scale.X;
	}

	private void SetupReferences()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		refs.levelManager.ResetSession += Destroy;
		refs.levelManager.SceneChanged += SceneChangeCleanup;
	}

	private void SetInitialValues()
	{
		_difficultySpeedMultiplier = refs.SelectedDifficulty.BallSpeedMultiplier;
		_difficultyArrowSpeed = refs.SelectedDifficulty.AngleSelectSpeed;
		Reset();
		UpdateSpeed();
	}

	private void Move(double delta)
	{
		float _moveSpeed = (float)delta * _speed;
		var collision = MoveAndCollide(Velocity * _moveSpeed);
		Bounce(collision);
		AdjustSpriteRotation();
	}

	private void Bounce(KinematicCollision2D collision)
	{
		if (collision == null)
		{
			return;
		}

		_animator.Play("bounce", 0, 1.5f);
		refs.audioController.PlayAudio(0);

		Velocity = Velocity.Bounce(collision.GetNormal());

		Breakable breakable = collision.GetCollider() as Breakable;
		breakable?.Damage(1);
	}

	private void AdjustSpriteRotation()
	{
		_sprite.Rotation = Velocity.Angle();
		_sprite.RotationDegrees += 90;
	}

	private void UpdateSpeed()
	{
		_speed = _baseSpeed * _difficultySpeedMultiplier * _speedMultiplier;
		_animator.Play("roll", 0, _difficultySpeedMultiplier * _speedMultiplier);
	}

	private void RotationSelect()
	{
		if (_startingRotation >= _maxReleaseAngle || _startingRotation <= -_maxReleaseAngle)
		{
			_increasingRotation = !_increasingRotation;
		}

		_startingRotation += _increasingRotation ? _difficultyArrowSpeed : -_difficultyArrowSpeed;
		_arrow.RotationDegrees = _startingRotation;
	}

	private void OnScreenExited()
	{
		if (refs.Balls.Count > 1)
		{
			Destroy();
		}
		else
		{
			Reset();
			refs.health.ChangeLives(-1);
		}
	}

	private void SceneChangeCleanup()
	{
		OnScreenExited();
	}

	private void Destroy()
	{
		refs.levelManager.ResetSession -= Destroy;
		refs.levelManager.SceneChanged -= SceneChangeCleanup;
		QueueFree();
	}
}
