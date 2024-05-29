using BoGK.Interfaces;
using BoGK.Models;
using Godot;

namespace BoGK.Gameplay
{
	public partial class Breakable : Node2D, IBreakable
	{
		[Export] protected bool _destroyOnDeath = true;
		[Export] protected int _pointValue = 1;
		[Export] protected int _maxHealth = 1;

		[Export] private double _pickupSpawnChance = 0.1f;
		[Export] private PackedScene[] _pickups;

		[Export] protected float _maxPositionOffset = 0.5f;
		[Export] protected double _shakeDuration = 0.25f;

		private Vector2 _defaultPosition;
		protected bool _isDead;
		protected int _health;

		protected string _defaultSpritePath;

		protected Sprite2D _sprite;
		[Export] protected Timer _timer;
		protected GameSystem.SessionController refs;

		public int MaxHealth
		{
			get { return _maxHealth; }
		}

		public override void _Ready()
		{
			_sprite = GetNode<Sprite2D>("Sprite");

			if (_timer != null)
			{
				_timer.Timeout += RestoreDefaultPosition;
			}

			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			SetInitialValues();
			AdjustSprite();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_timer == null)
			{
				return;
			}

			AnimateHitReaction();
		}

		public virtual void Damage(int value)
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

			_timer?.Start(_shakeDuration);
			AdjustSprite();
		}

		protected virtual void SetInitialValues()
		{
			_defaultPosition = _sprite.Position;
			_health = _maxHealth;
			_isDead = _health <= 0;
			_defaultSpritePath = _sprite.Texture.ResourcePath;
			ApplySpriteVariant();
		}

		protected virtual void AdjustSprite()
		{
			_sprite.Frame = (_sprite.Hframes > _maxHealth - _health) ? _maxHealth - _health : 0;
		}

		protected void ApplySpriteVariant()
		{
			string breakableName = _defaultSpritePath.Split('/')[^1].Replace(".png", "");
			BreakableVariant variant = refs.settings.FindVariant(breakableName);

			switch (variant.SpriteVariant)
			{
				case 1:
					_sprite.Texture = ResourceLoader.Load<Texture2D>(GameSystem.HelperMethods.ReplaceSpritePalettePath(_defaultSpritePath, refs.settings.InteractableColorPalette).Replace(".png", "_rim.png"));
					_sprite.Modulate = new Color(1, 1, 1, 1);
					break;

				case 2:
					_sprite.Texture = ResourceLoader.Load<Texture2D>(_defaultSpritePath.Replace(refs.settings.InteractableColorPalette, "custom"));
					_sprite.Modulate = variant.CustomColor;
					break;

				default:
					_sprite.Texture = ResourceLoader.Load<Texture2D>(GameSystem.HelperMethods.ReplaceSpritePalettePath(_defaultSpritePath, refs.settings.InteractableColorPalette));
					_sprite.Modulate = new Color(1, 1, 1, 1);
					break;
			}
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
				GetNode("../../..").CallDeferred(Node.MethodName.AddChild, pickup);
				pickup.Position = Position;
			}
		}

		private void AnimateHitReaction()
		{
			if (_timer.TimeLeft > 0)
			{
				float horizontalOffset = (float)GD.RandRange(0.0, 1.0) * _maxPositionOffset;
				float verticalOffset = (float)GD.RandRange(0.0, 1.0) * _maxPositionOffset;

				_sprite.Position = _defaultPosition + new Vector2(horizontalOffset, verticalOffset);
			}
		}

		private void RestoreDefaultPosition()
		{
			_sprite.Position = _defaultPosition;
		}

		protected virtual void Destroy()
		{
			SpawnPickup();
			_isDead = true;
			refs.gameScore.ChangeScore(_pointValue);

			if (_destroyOnDeath)
			{
				QueueFree();
			}
		}
	}
}