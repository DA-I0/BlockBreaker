using BoGK.Interfaces;
using Godot;

namespace BoGK.Gameplay
{
	public enum EnemyState { idle, moving, hurt };
	public enum MovementType { random, pathSequence, pathPingPong };

	public partial class Enemy : CharacterBody2D, IBreakable
	{
		[Export] protected EnemyState _state;
		[Export] protected float _speed;
		[Export] protected float _minMoveDistance;
		[Export] protected float _maxMoveDistance;
		[Export] protected bool _randomizeThinkingDuration = false;
		[Export] protected double _maxThinkingDuration;
		[Export] protected float _maxHorizontalPosition = 94;
		[Export] protected float _maxVerticalPosition = 70;
		[Export] protected MovementType _movementType;
		[Export] protected Node2D[] _path;

		[Export] protected Timer _movementTimer;
		[Export] protected AnimationPlayer _animator;

		// breakable stuff
		[Export] protected int _pointValue = 1;
		[Export] protected int _maxHealth = 1;
		[Export] protected double _invulnerabilityDuration = 1f;
		[Export] protected Timer _invulnerabilityTimer;

		[Export] protected double _pickupSpawnChance = 0.1f;
		[Export] protected PackedScene[] _pickups;

		[Export] protected float _maxPositionOffset = 0.5f;

		protected bool _isDead;
		protected int _health;

		protected string _defaultSpritePath;
		// breakable stuff - end

		protected EnemyState _lastState;
		protected Vector2 _newDestination;
		[Export] protected Vector2 _moveDirection = new Vector2(0, 1);
		protected float _moveDistance;
		protected int _nextPathIndex = -1;
		protected int _pathDirection = 1;

		protected Sprite2D _sprite;
		protected GameSystem.SessionController refs;

		public int MaxHealth
		{
			get { return _maxHealth; }
		}

		public override void _Ready()
		{
			_sprite = GetNode<Sprite2D>("Sprite");

			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			SetInitialValues();
			ApplyPalette();
			_lastState = _state;
			AnimateMovement(0f);
		}

		public override void _PhysicsProcess(double delta)
		{
			Move(delta);
		}

		public virtual void BraceOnContact(Node2D attacker)
		{
			if (_state == EnemyState.hurt)
			{
				return;
			}

			PauseMovement();
		}

		protected virtual void TriggerDamageReaction()
		{
			_lastState = _state;
			_state = EnemyState.hurt;
			AnimateDamage();
			_invulnerabilityTimer.Stop();
			_invulnerabilityTimer.Start(_invulnerabilityDuration);
		}

		protected virtual void PauseMovement()
		{
			RestorePreviousState();

			_lastState = _state;
			_state = EnemyState.idle;
			AnimateMovement(0f);
			_invulnerabilityTimer.Stop();
			_invulnerabilityTimer.Start(0.1f);
		}

		protected void Move(double delta)
		{
			if (_state == EnemyState.moving && _movementTimer.TimeLeft <= 0)
			{
				if (CheckTargetDistance() > 1f)
				{
					Vector2 newVelocity = Position.DirectionTo(_newDestination) * (float)delta * _speed;
					KinematicCollision2D collision = MoveAndCollide(newVelocity, false, 0.01f);
					AnimateMovement();

					if (collision != null && _movementType == MovementType.random)
					{
						SelectDestination();
					}
				}
				else
				{
					_animator.Stop();
					StartDestinationPlanning();
				}
			}
		}

		protected void SelectDestination()
		{
			if (_movementType == MovementType.random)
			{
				SelectRandomDestinationPoint();
			}
			else
			{
				SelectPathDestinationPoint();
			}
		}

		protected void SelectRandomDestinationPoint()
		{
			int directionModifier = 0;

			while (directionModifier == 0)
			{
				directionModifier = GD.RandRange(-1, 1);
			}

			_moveDirection = (GD.RandRange(0, 1) == 0) ? new Vector2(directionModifier, 0) : new Vector2(0, directionModifier);
			_moveDistance = (float)GD.RandRange(_minMoveDistance, _maxMoveDistance);

			_newDestination = Position + _moveDirection * _moveDistance;
			CheckDestinationLimits();
		}

		protected void CheckDestinationLimits()
		{
			if (_newDestination.X >= _maxHorizontalPosition)
			{
				_newDestination.X = _maxHorizontalPosition - 1;
			}

			if (_newDestination.X <= -_maxHorizontalPosition)
			{
				_newDestination.X = -_maxHorizontalPosition + 1;
			}

			if (_newDestination.Y >= _maxVerticalPosition)
			{
				_newDestination.Y = _maxVerticalPosition - 1;
			}
		}

		protected void SelectPathDestinationPoint()
		{
			if (_path == null || _path.Length < 1)
			{
				return;
			}

			_nextPathIndex += _pathDirection;
			CheckPathPoints();

			_newDestination = _path[_nextPathIndex].Position;
			_moveDirection = Position.DirectionTo(_newDestination);
		}

		protected void CheckPathPoints()
		{
			if (_movementType == MovementType.pathSequence)
			{
				if (_nextPathIndex >= _path.Length)
				{
					_nextPathIndex = 0;
					return;
				}
			}

			if (_movementType == MovementType.pathPingPong)
			{
				if (_nextPathIndex >= _path.Length)
				{
					_nextPathIndex = _path.Length - 2;
					_pathDirection = -1;
					return;
				}

				if (_nextPathIndex < 0)
				{
					_nextPathIndex = 1;
					_pathDirection = 1;
				}
			}
		}

		protected float CheckTargetDistance()
		{
			return Position.DistanceTo(_newDestination);
		}

		protected void StartDestinationPlanning()
		{
			if (_randomizeThinkingDuration)
			{
				_movementTimer.Start(GD.RandRange(0, _maxThinkingDuration));
			}
			else
			{
				_movementTimer.Start(_maxThinkingDuration);
			}
		}

		protected void AnimateMovement(float customSpeed = 1f)
		{

			switch (SpriteDirection())
			{
				case Vector2(0f, -1f):
					_animator.Play("walk_up", -1, customSpeed);
					break;

				case Vector2(-1f, 0f):
					_animator.Play("walk_left", -1, customSpeed);
					break;

				case Vector2(1f, 0f):
					_animator.Play("walk_right", -1, customSpeed);
					break;

				default:
					_animator.Play("walk_down", -1, customSpeed);
					break;
			}
		}

		protected void AnimateDamage()
		{
			switch (SpriteDirection())
			{
				case Vector2(0f, -1f):
					_animator.Play("damage_up");
					break;

				case Vector2(-1f, 0f):
					_animator.Play("damage_left");
					break;

				case Vector2(1f, 0f):
					_animator.Play("damage_right");
					break;

				default:
					_animator.Play("damage_down");
					break;
			}
		}

		protected Vector2 SpriteDirection()
		{
			return new Vector2(Mathf.RoundToInt(_moveDirection.X), Mathf.RoundToInt(_moveDirection.Y));
		}

		protected void RestorePreviousState()
		{
			AnimateMovement(0f);
			_state = _lastState;
		}

		public virtual void Damage(int value)
		{
			if (_isDead || _state == EnemyState.hurt)
			{
				return;
			}

			_health -= value;

			if (_health <= 0)
			{
				Destroy();
			}

			RestorePreviousState();
			TriggerDamageReaction();
		}

		protected virtual void SetInitialValues()
		{
			_health = _maxHealth;
			_isDead = _health <= 0;
			_defaultSpritePath = _sprite.Texture.ResourcePath;
			_newDestination = Position;
		}

		protected void SpawnPickup()
		{
			if (_pickups == null || _isDead)
			{
				return;
			}

			double dropRandomization = GD.RandRange(0.0, 1.0);

			if (dropRandomization <= _pickupSpawnChance)
			{
				int pickupType = GD.RandRange(0, _pickups.Length - 1);
				Area2D pickup = _pickups[pickupType].Instantiate<Area2D>();
				GetNode("../../..").CallDeferred(Node.MethodName.AddChild, pickup);
				pickup.Position = Position;
			}
		}

		protected void ApplyPalette()
		{
			_sprite.Texture = ResourceLoader.Load<Texture2D>(GameSystem.HelperMethods.ReplaceSpritePalettePath(_defaultSpritePath, refs.settings.InteractableColorPalette));
		}

		protected virtual void Destroy()
		{
			SpawnPickup();
			_isDead = true;
			refs.gameScore.ChangeScore(_pointValue);
			QueueFree();
		}
	}
}
