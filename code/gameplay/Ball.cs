using Godot;

public enum BallMode { idle, angleSelection, moving, frozen, spinning };

public partial class Ball : CharacterBody2D
{
	private const float MaxBallSpeedMultiplier = 1.5f;
	private const float MinBallSpeedMultiplier = 0.5f;

	private BallMode _ballMode = BallMode.idle;

	[Export] private int _maxReleaseAngle = 60;
	[Export] private float _baseSpeed = 10;
	[Export] private float _boostDrag = 0.01f;
	[Export] private float _idlePositionOffset = 7f;

	private float _speedMultiplier = 1f;
	private float _boostMultiplier = 1f;

	private bool _increasingRotation = true;
	private float _startingRotation = 0;

	private float _combinedSpeedMultiplier;

	[Export] private Sprite2D _sprite;
	[Export] private Sprite2D _arrow;
	[Export] private Timer _arrowTimer;
	[Export] private AnimationPlayer _animator;
	[Export] private CpuParticles2D _particles;

	private SessionController refs;

	public BallMode BallMode
	{
		get { return _ballMode; }
		set
		{
			_ballMode = value;
			ApplyModeValues();
		}
	}

	public float SpeedMultiplier
	{
		get { return _speedMultiplier; }
		set
		{
			_speedMultiplier = value;
			CheckSpeedMultiplierLimits();
			UpdateSpeed();
		}
	}

	public float Size
	{
		get { return Scale.X; }
	}

	public override void _Input(InputEvent @event)
	{
		if (refs.CurrentGameState != GameState.gameplay)
		{
			return;
		}

		if (@event.IsActionPressed("game_play"))
		{
			switch (_ballMode)
			{
				case BallMode.idle:
					BallMode = BallMode.angleSelection;
					_arrowTimer.Start(0.001f);
					break;

				case BallMode.angleSelection:
					Release();
					break;

				default:
					break;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (refs.CurrentGameState != GameState.gameplay)
		{
			return;
		}

		if (_ballMode == BallMode.moving)
		{
			Move(delta);
			ReduceTempSpeedMultiplier();
			return;
		}

		if (_ballMode == BallMode.idle)
		{
			Position = (refs.paddle != null) ? new Vector2(refs.paddle.Position.X, refs.paddle.Position.Y - _idlePositionOffset) : Vector2.Zero;
		}
	}

	public void SetupReferences(SessionController sessionController)
	{
		refs = sessionController;
		refs.levelManager.ResetSession += Destroy;
		refs.levelManager.SceneChanged += SceneChangeCleanup;
		refs.health.ResetElements += Reset;
	}

	public void SetInitialValues(SessionController sessionController, Ball sourceBall = null, float angleChange = 0)
	{
		SetupReferences(sessionController);
		_animator.AnimationSetNext("bounce", "roll");


		if (sourceBall == null)
		{
			Position = refs.paddle.Position;
			Reset();
			return;
		}
		else
		{
			Position = sourceBall.Position;
			ChangeSize(sourceBall.Size);
			SpeedMultiplier = sourceBall.SpeedMultiplier;
			Release();
			ChangeRotation(sourceBall.RotationDegrees + angleChange);
		}
	}

	public void Reset()
	{
		SpeedMultiplier = 1f;
		ChangeSize(1f);
		StateReset();
	}

	public void SpawnCopy(float angleChange)
	{
		Ball newBall = (Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate();
		newBall.CallDeferred("SetInitialValues", refs, this, angleChange);
		GetParent().CallDeferred(Node.MethodName.AddChild, newBall);
	}

	public void AddVelocity(Vector2 velocity)
	{
		// Velocity += velocity * 0.1f;
	}

	public void ChangeSpeedMultiplier(float value)
	{
		SpeedMultiplier += value;
	}

	public void ChangeTempSpeedMultiplier(float value)
	{
		_boostMultiplier = value;
		UpdateSpeed();
	}

	public void ChangeSize(float value)
	{
		Scale = new Vector2(value, value);
		float arrowScale = (value == 1) ? 1 : (1 / value);
		_arrow.Scale = new Vector2(arrowScale, arrowScale);
	}

	public void ChangeRotation(float value)
	{
		Velocity = Velocity.Rotated(Mathf.DegToRad(value));
	}

	public void ChangeRotationTo(float value)
	{
		float rotationDifference = Mathf.DegToRad(value) - Velocity.Angle();
		Velocity = Velocity.Rotated(rotationDifference);
	}

	public void StateReset()
	{
		BallMode = BallMode.idle;
	}

	private void Release()
	{
		Velocity = new Vector2(0, -_baseSpeed).Rotated(_arrow.Rotation);
		_arrowTimer.Stop();
		BallMode = BallMode.moving;
	}

	private void Move(double delta)
	{
		float _moveSpeed = (float)delta * _combinedSpeedMultiplier;
		KinematicCollision2D collision = MoveAndCollide(Velocity * _moveSpeed);
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

	private void CheckSpeedMultiplierLimits()
	{
		if (_speedMultiplier > MaxBallSpeedMultiplier)
		{
			_speedMultiplier = MaxBallSpeedMultiplier;
		}

		if (_speedMultiplier < MinBallSpeedMultiplier)
		{
			_speedMultiplier = MinBallSpeedMultiplier;
		}
	}

	private void UpdateSpeed()
	{
		_combinedSpeedMultiplier = _baseSpeed * refs.SelectedDifficulty.BallSpeedMultiplier * _speedMultiplier * _boostMultiplier;
		_animator.Play("roll", 0, _combinedSpeedMultiplier);
	}

	private void ReduceTempSpeedMultiplier()
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

	private void RotateArrow()
	{
		if (_startingRotation >= _maxReleaseAngle || _startingRotation <= -_maxReleaseAngle)
		{
			_increasingRotation = !_increasingRotation;
		}

		_startingRotation += _increasingRotation ? refs.SelectedDifficulty.AngleSelectSpeed : -refs.SelectedDifficulty.AngleSelectSpeed;
		_arrow.RotationDegrees = _startingRotation;
	}

	private void AdjustSpriteRotation()
	{
		_sprite.Rotation = Velocity.Angle() + Mathf.DegToRad(90);
	}

	private void ApplyModeValues()
	{
		_arrow.Visible = false;
		_particles.Emitting = false;

		switch (_ballMode)
		{
			case BallMode.angleSelection:
				_startingRotation = 0;
				_arrow.Visible = true;
				_increasingRotation = true;
				refs.paddle.SetPaddleState(PaddleState.locked);
				break;

			case BallMode.moving:
				refs.paddle.SetPaddleState(PaddleState.idle);
				_animator.Play("roll");
				_particles.Emitting = true;
				break;

			case BallMode.spinning:
				_animator.Play("spin");
				break;

			default:
				Velocity = Vector2.Zero;
				ChangeTempSpeedMultiplier(1);
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
			if (!levelChange)
			{
				refs.health.ChangeLives(-1);
				// Reset();
			}
			else
			{
				StateReset();
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
