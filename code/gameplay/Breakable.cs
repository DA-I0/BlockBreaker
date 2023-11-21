using Godot;

public partial class Breakable : StaticBody2D
{
	[Export] protected int _pointValue = 1;
	[Export] protected int _maxHealth = 1;
	[Export] protected Texture2D[] _sprites;

	[Export] protected float _maxPositionOffset = 0.5f;
	[Export] protected double _shakeDuration = 0.25f;

	private Vector2 _defaultPosition;
	protected int _health;

	protected Sprite2D _sprite;
	[Export] protected Timer _timer;
	protected SessionController refs;

	public int MaxHealth
	{
		get { return _maxHealth; }
	}

	public override void _Ready()
	{
		_sprite = GetNode("Sprite") as Sprite2D;

		refs = GetNode("/root/GameController") as SessionController;
		SetInitialValues();
		AdjustSprite();
	}

	public override void _Process(double delta)
	{
		if (_timer == null)
		{
			return;
		}

		if (_timer.TimeLeft > 0)
		{
			float horizontalOffset = (float)GD.RandRange(0.0, 1.0) * _maxPositionOffset;
			float verticalOffset = (float)GD.RandRange(0.0, 1.0) * _maxPositionOffset;

			_sprite.Position = _defaultPosition + new Vector2(horizontalOffset, verticalOffset);
		}
		else
		{
			_sprite.Position = _defaultPosition;
		}
	}

	public virtual void Damage(int value)
	{
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
	}

	protected virtual void AdjustSprite()
	{
		if (_sprite != null && _sprites.Length > 0)
		{
			int spriteIndex = (_health <= 0) ? 0 : _maxHealth - _health;
			_sprite.Texture = _sprites[spriteIndex];
		}
	}

	protected virtual void Destroy()
	{
		refs.gameScore.ChangeScore(_pointValue);
		QueueFree();
	}
}
