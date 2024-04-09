using BoGK.Interfaces;
using Godot;

public enum EnemyState { idle, standby, moving, hurt };

namespace BoGK.Gameplay
{
	public partial class Enemy : CharacterBody2D, IBreakable
	{
		[Export] private EnemyState _state;
		[Export] private float _speed;
		[Export] private float _minMoveDistance;
		[Export] private float _maxMoveDistance;
		[Export] private float _maxThinkingDuration;
		[Export] private float _maxHorizontalPosition = 94;
		[Export] private float _maxVerticalPosition = 80;
		[Export] private Timer _movementTimer;
		[Export] private AnimationPlayer _animator;

		// breakable stuff
		[Export] protected int _pointValue = 1;
		[Export] protected int _maxHealth = 1;
		[Export] protected double _invulnerabilityDuration = 1f;
		[Export] protected Timer _invulnerabilityTimer;

		[Export] private double _pickupSpawnChance = 0.1f;
		[Export] private PackedScene[] _pickups;

		[Export] protected float _maxPositionOffset = 0.5f;

		private Vector2 _defaultPosition;
		private bool _isDead;
		protected int _health;

		protected string _defaultSpritePath;
		// breakable stuff - end

		private EnemyState _lastState;
		private Vector2 _newDestination;
		[Export] private Vector2 _moveDirection = new Vector2(0, 1);
		private float _moveDistance;

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
			_lastState = _state;
			AnimateMovement(0f);
		}

		public override void _PhysicsProcess(double delta)
		{
			Move(delta);
		}

		protected virtual void TriggerDamageReaction()
		{
			_lastState = _state;
			_state = EnemyState.hurt;
			AnimateDamage();
			_invulnerabilityTimer.Start(_invulnerabilityDuration);
		}

		private void Move(double delta)
		{
			if (_state == EnemyState.moving)
			{
				if (CheckTargetDistance() > 1f)
				{
					Vector2 newVelocity = Position.DirectionTo(_newDestination) * (float)delta * _speed;
					KinematicCollision2D collision = MoveAndCollide(newVelocity, false, 0.01f);
					AnimateMovement();

					if (collision != null)
					{
						SelectDestination();
					}
				}
				else
				{
					_animator.Stop();
					RandomizeThinkingDelay();
				}
			}
		}

		private void SelectDestination()
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

		private void CheckDestinationLimits()
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

		private float CheckTargetDistance()
		{
			return Position.DistanceTo(_newDestination);
		}

		private void RandomizeThinkingDelay()
		{
			_movementTimer.Start(GD.RandRange(0, _maxThinkingDuration));
		}

		private void AnimateMovement(float customSpeed = 1f)
		{
			switch (_moveDirection)
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

		private void AnimateDamage()
		{
			switch (_moveDirection)
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

		private void RestorePreviousState()
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

			TriggerDamageReaction();
			RandomizeThinkingDelay();
		}

		protected virtual void SetInitialValues()
		{
			_defaultPosition = _sprite.Position;
			_health = _maxHealth;
			_isDead = _health <= 0;
			_defaultSpritePath = _sprite.Texture.ResourcePath;
			_newDestination = Position;
		}

		private void SpawnPickup()
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
				GetNode("../../..").CallDeferred("add_child", pickup);
				pickup.Position = Position;
			}
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
