using Godot;

public enum BallMode { idle, angleSelection, moving, frozen, spinning };

public partial class Ball : CharacterBody2D
{
	[Export] private int _maxReleaseAngle = 60;
	[Export] private float _baseSpeed = 10;
	[Export] private float _boostDrag = 0.01f;
	[Export] private float _idlePositionOffset = 7f;

	private BallMode _ballMode = BallMode.idle;

	private float _difficultyArrowSpeed = 1f;
	private float _difficultySpeedMultiplier = 1f;
	private float _speedMultiplier = 1f;
	private float _boostMultiplier = 0f;

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
		set
		{
			_ballMode = value;
			AdjustToState();
		}
	}

	public float SpeedMultiplier
	{
		get { return _speedMultiplier; }
	}

	public override void _Ready()
	{
		SetupReferences();
		SetInitialValues();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_ballMode == BallMode.moving)
		{
			Move(delta);
			ReduceTempBoost();
			return;
		}

		if (_ballMode == BallMode.idle)
		{
			Position = (refs.paddle != null) ? new Vector2(refs.paddle.Position.X, refs.paddle.Position.Y - _idlePositionOffset) : Vector2.Zero;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("game_play"))
		{
			switch (_ballMode)
			{
				case BallMode.idle:
					BallMode = BallMode.angleSelection;
					_arrowTimer.Start(0.001f);
					break;

				case BallMode.angleSelection:
					PlayBall();
					break;

				case BallMode.moving:
					// Shake();
					break;

				default:
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
		UpdateSpeed();
		Velocity = new Vector2(0, -_speed).Rotated(_arrow.Rotation);
		_arrowTimer.Stop();
		BallMode = BallMode.moving;
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
		BallMode = BallMode.idle;
	}

	public float GetBallSpeed()
	{
		return _speedMultiplier;
	}

	public float GetBallSize()
	{
		return _collider.Scale.X;
	}

	public void SetTempBoost(float boostMultiplier)
	{
		_boostMultiplier = boostMultiplier;
		UpdateSpeed();
	}

	public void RotateBall(float angleChange)
	{
		Velocity = Velocity.Rotated(Mathf.DegToRad(angleChange));
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
		_animator.AnimationSetNext("bounce", "roll");
		Reset();
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

		if (collision.GetCollider() == refs.paddle)
		{
			refs.paddle.ApplyPaddleEffect(this);
		}
	}

	private void AdjustSpriteRotation()
	{
		_sprite.Rotation = Velocity.Angle();
		_sprite.RotationDegrees += 90;
	}

	private void UpdateSpeed()
	{
		_speed = _baseSpeed * _difficultySpeedMultiplier * _speedMultiplier * _boostMultiplier;
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

	private void ReduceTempBoost()
	{
		if (_boostMultiplier > 1)
		{
			_boostMultiplier -= _boostDrag;
			UpdateSpeed();
		}
		else
		{
			_boostMultiplier = 1;
		}
	}

	private void AdjustToState()
	{
		_arrow.Visible = false;

		switch (_ballMode)
		{
			case BallMode.angleSelection:
				_arrow.Visible = true;
				refs.paddle.SetPaddleState(PaddleState.locked);
				break;

			case BallMode.moving:
				refs.paddle.SetPaddleState(PaddleState.idle);
				_animator.Play("roll");
				break;

			case BallMode.spinning:
				_animator.Play("spin");
				break;

			default:
				SetTempBoost(1);
				_animator.Play("idle");
				break;
		}
	}

	private void OnScreenExited(bool levelChange = false)
	{
		if (refs.Balls.Count > 1)
		{
			Destroy();
		}
		else
		{
			Reset();

			if (!levelChange)
			{
				refs.health.ChangeLives(-1);
			}
		}
	}

	private void SceneChangeCleanup()
	{
		OnScreenExited(true);
	}

	private void Destroy()
	{
		refs.levelManager.ResetSession -= Destroy;
		refs.levelManager.SceneChanged -= SceneChangeCleanup;
		QueueFree();
	}
}
