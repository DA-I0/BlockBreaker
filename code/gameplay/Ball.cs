using BoGK.GameSystem;
using BoGK.Interfaces;
using Godot;

public enum BallMode { idle, angleSelection, moving, frozen, spinning };

namespace BoGK.Gameplay
{
	public partial class Ball : CharacterBody2D
	{
		private const float MaxBallSpeedMultiplier = 1.5f;
		private const float MinBallSpeedMultiplier = 0.5f;
		private const float MaxAdvancingSpeedMultiplier = 2f;
		private const float MinAdvancingSpeedMultiplier = 1f;
		private const int MaxTrailCurvePoints = 10;
		private const string DefaultSprite = "res://assets/sprites/animations/ball.png";
		private const string PowerSprite = "res://assets/sprites/animations/power_ball.png";

		private BallMode _ballMode = BallMode.idle;

		[Export] private int _maxReleaseAngle = 60;
		[Export] private float _baseSpeed = 10;
		[Export] private float _boostDrag = 0.01f;
		[Export] private float _idlePositionOffset = 7f;

		private float _speedMultiplier = 1f;
		private float _boostMultiplier = 1f;
		private float _advancingSpeedMultiplier = 1f;
		private int _lastSpeedUpdateComboValue = 0;
		private bool _isPowerBall = false;

		private bool _increasingRotation = true;
		private float _startingRotation = 0;

		private float _combinedSpeed;
		private Curve2D _speedTrailCurve = new Curve2D();


		[Export] private Sprite2D _sprite;
		[Export] private Sprite2D _arrow;
		[Export] private Timer _arrowTimer;
		[Export] private AnimationPlayer _animator;
		[Export] private CpuParticles2D _particles;
		[Export] private Line2D _speedTrail;

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

			if (HelperMethods.IsActiveInputDevice(refs, @event) && @event.IsActionPressed("game_play"))
			{
				switch (_ballMode)
				{
					case BallMode.idle:
						BallMode = BallMode.angleSelection;
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

			if (refs.SelectedDifficulty.AdvancingSpeed)
			{
				refs.gameScore.ScoreChanged += AdvancingSpeed;
			}
		}

		public void SetInitialValues(SessionController sessionController, Ball sourceBall = null, float angleChange = 0)
		{
			SetupReferences(sessionController);

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
			_advancingSpeedMultiplier = MinAdvancingSpeedMultiplier;
			SpeedMultiplier = 1f;
			ChangeSize(1f);
			SetPowerBallState(false);
			StateReset();
		}

		public void SpawnCopy(float angleChange)
		{
			Ball newBall = (Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate();
			newBall.CallDeferred("SetInitialValues", refs, this, angleChange);
			GetParent().CallDeferred(Node.MethodName.AddChild, newBall);
		}

		public void ChangeTempSpeedMultiplier(float value)
		{
			_boostMultiplier += value;
			UpdateSpeed();
		}

		public void SetTempSpeedMultiplier(float value)
		{
			_boostMultiplier = value;
			UpdateSpeed();
		}

		public void ChangeSize(float value)
		{
			Scale = new Vector2(value, value);
			float arrowScale = (value == 1) ? 1 : (1 / value);
			_arrow.Scale = new Vector2(arrowScale, arrowScale);

			_speedTrail.WidthCurve.SetPointValue(1, value);
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

		public void SetPowerBallState(bool active)
		{
			_isPowerBall = active;
			AdjustPowerVisuals();
		}

		private void Release()
		{
			Velocity = new Vector2(0, -_baseSpeed).Rotated(_arrow.Rotation);
			_arrowTimer.Stop();
			BallMode = BallMode.moving;
		}

		private void Move(double delta)
		{
			float _moveSpeed = (float)delta * _combinedSpeed;
			KinematicCollision2D collision = MoveAndCollide(Velocity * _moveSpeed, false, 0.01f);
			Bounce(collision);
			AdjustSpriteRotation();
			UpdateSpeedTrail();
		}

		private void Bounce(KinematicCollision2D collision)
		{
			if (collision == null)
			{
				return;
			}

			refs.audioController.PlayAudio(0);

			IBreakable breakable = collision.GetCollider() as IBreakable;
			DamageCollider(breakable);

			if (!_isPowerBall || breakable == null)
			{
				Velocity = Velocity.Bounce(collision.GetNormal());
				_animator.Stop();
				_animator.Play("bounce", 0, 1.5f);
			}

			try
			{
				((SecondaryCollider)collision.GetCollider())?.HitContact(this);
			}
			catch { }

			try
			{
				((BasePaddle)collision.GetCollider())?.ApplyPaddleEffect(this);
			}
			catch { }
		}

		private void DamageCollider(IBreakable breakable)
		{
			if (breakable == null)
			{
				return;
			}

			if (_isPowerBall)
			{
				breakable.Damage(999);
			}
			else
			{
				breakable?.Damage(1);
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

		private void CheckAdvancingSpeedMultiplierLimits()
		{
			if (_advancingSpeedMultiplier > MaxAdvancingSpeedMultiplier)
			{
				_advancingSpeedMultiplier = MaxAdvancingSpeedMultiplier;
			}

			if (_advancingSpeedMultiplier < MinAdvancingSpeedMultiplier)
			{
				_advancingSpeedMultiplier = MinAdvancingSpeedMultiplier;
			}
		}

		private void UpdateSpeed()
		{
			float _combinedSpeedMultiplier = refs.SelectedDifficulty.BallSpeedMultiplier * _speedMultiplier * _boostMultiplier * _advancingSpeedMultiplier;
			_combinedSpeed = _baseSpeed * _combinedSpeedMultiplier;
			_animator.SpeedScale = _combinedSpeedMultiplier;

			ToggleSpeedTrail();
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

		private void AdvancingSpeed(int score, int scoreMultiplier, int combo)
		{
			if (_lastSpeedUpdateComboValue < combo)
			{
				_advancingSpeedMultiplier += 0.02f;
				_lastSpeedUpdateComboValue = combo;
				CheckAdvancingSpeedMultiplierLimits();
				UpdateSpeed();
			}
		}

		private void RotateArrow()
		{
			if (_startingRotation >= _maxReleaseAngle || _startingRotation <= -_maxReleaseAngle)
			{
				_increasingRotation = !_increasingRotation;
			}

			_startingRotation += _increasingRotation ? refs.SelectedDifficulty.AngleSelectSpeed : -refs.SelectedDifficulty.AngleSelectSpeed;
			_arrow.RotationDegrees = _startingRotation + _sprite.RotationDegrees;
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
					_arrowTimer.Start(0.001f);
					_animator.Pause();
					refs.paddle.SetPaddleState(PaddleState.locked);
					break;

				case BallMode.moving:
					refs.paddle.SetPaddleState(PaddleState.idle);
					UpdateSpeed();
					_animator.Play("roll");
					_particles.Emitting = true;
					break;

				case BallMode.spinning:
					_animator.Play("spin", 0, 1 / _animator.SpeedScale);
					break;

				default:
					Velocity = Vector2.Zero;
					SetTempSpeedMultiplier(1);
					_animator.Play("idle");
					_speedTrailCurve.ClearPoints();
					break;
			}
		}

		private void UpdateSpeedTrail()
		{
			_speedTrailCurve.AddPoint(Position);

			if (_speedTrailCurve.GetBakedPoints().Length > MaxTrailCurvePoints)
			{
				_speedTrailCurve.RemovePoint(0);
			}

			_speedTrail.Points = _speedTrailCurve.GetBakedPoints();
		}

		private void ToggleSpeedTrail()
		{
			_speedTrail.Visible = (_speedMultiplier * _boostMultiplier * _advancingSpeedMultiplier > 1) && _ballMode == BallMode.moving;
		}

		private void AdjustPowerVisuals()
		{
			if (_isPowerBall)
			{
				_sprite.Texture = ResourceLoader.Load<Texture2D>(PowerSprite);
			}
			else
			{
				_sprite.Texture = ResourceLoader.Load<Texture2D>(DefaultSprite);
			}
		}

		private void OnScreenExited()
		{
			if (refs.Balls.Count > 1)
			{
				Destroy();
			}
			else
			{
				refs.health.ChangeLives(-1);
			}
		}

		private void SceneChangeCleanup()
		{
			if (refs.Balls[0] == this)
			{
				StateReset();
			}
			else
			{
				Destroy();
			}
		}

		private void Destroy()
		{
			GetParent().RemoveChild(this);
			refs.levelManager.ResetSession -= Destroy;
			refs.levelManager.SceneChanged -= SceneChangeCleanup;
			refs.health.ResetElements -= Reset;

			if (refs.SelectedDifficulty.AdvancingSpeed)
			{
				refs.gameScore.ScoreChanged -= AdvancingSpeed;
			}

			QueueFree();
		}
	}
}