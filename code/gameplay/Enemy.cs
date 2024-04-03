using BoGK.Interfaces;
using Godot;

public enum EnemyState { idle, standby, moving };

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
		// breakable stuff
		[Export] protected int _pointValue = 1;
		[Export] protected int _maxHealth = 1;

		[Export] private double _pickupSpawnChance = 0.1f;
		[Export] private PackedScene[] _pickups;

		[Export] protected float _maxPositionOffset = 0.5f;
		[Export] protected double _invulnerabilityDuration = 0.25f;

		[Export] protected Timer _invulnerabilityTimer;

		private Vector2 _defaultPosition;
		private bool _isDead;
		protected int _health;

		protected string _defaultSpritePath;
		// breakable stuff - end
		private Vector2 _newDestination;
		private Vector2 _moveDirection;
		private float _moveDistance;

		public override void _Ready()
		{
			_sprite = GetNode<Sprite2D>("Sprite");

			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			SetInitialValues();
			AdjustSprite();
		}

		public override void _PhysicsProcess(double delta)
		{
			Move(delta);
		}

		protected virtual void AdjustSprite() // rename to ShowDamage or something
		{
			// replace with animations/animation rows as frames are needed for walking anims
			_sprite.Frame = (_health <= 0) ? 0 : _maxHealth - _health;
		}

		private void Move(double delta)
		{
			if (_state == EnemyState.moving)
			{
				if (CheckTargetDistance() > 1f)
				{
					Vector2 newVelocity = Position.DirectionTo(_newDestination) * (float)delta * _speed;
					KinematicCollision2D collision = MoveAndCollide(newVelocity, false, 0.01f);

					if (collision != null)
					{
						SelectDestination();
					}
				}
				else
				{
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

			GD.Print($"New destination: {_newDestination} -- Direction: {_moveDirection}");
		}

		private float CheckTargetDistance()
		{
			return Position.DistanceTo(_newDestination);
		}

		private void RandomizeThinkingDelay()
		{
			_movementTimer.Start(GD.RandRange(0, _maxThinkingDuration));
		}
		//------------------------------

		protected Sprite2D _sprite;
		protected GameSystem.SessionController refs;

		public int MaxHealth
		{
			get { return _maxHealth; }
		}

		public virtual void Damage(int value) // stop movement, make invulnerable for a moment
		{
			if (_isDead)
			{
				return;
			}

			_health -= value;

			if (_health <= 0)
			{
				Destroy();
			}

			// _timer?.Start(_shakeDuration);
			AdjustSprite();
			RandomizeThinkingDelay();
		}

		protected virtual void SetInitialValues()
		{
			_defaultPosition = _sprite.Position;
			_health = _maxHealth;
			_isDead = _health <= 0;
			_defaultSpritePath = _sprite.Texture.ResourcePath;
			ApplySpriteVariant();
			_newDestination = Position;
		}

		private void ApplySpriteVariant()
		{
			/*
						string breakableName = _defaultSpritePath.Split('/')[^1].Replace(".png", "");
						BreakableVariant variant = refs.settings.FindVariant(breakableName);

						switch (variant.SpriteVariant)
						{
							case 1:
								_sprite.Texture = ResourceLoader.Load<Texture2D>(_defaultSpritePath.Replace(".png", "_rim.png"));
								_sprite.Modulate = new Color(1, 1, 1, 1);
								break;

							case 2:
								_sprite.Texture = ResourceLoader.Load<Texture2D>(_defaultSpritePath.Replace(".png", "_alt.png"));
								_sprite.Modulate = variant.CustomColor;
								break;

							default:
								_sprite.Texture = ResourceLoader.Load<Texture2D>(_defaultSpritePath);
								_sprite.Modulate = new Color(1, 1, 1, 1);
								break;
						}
						*/
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
