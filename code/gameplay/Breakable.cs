using Godot;

public partial class Breakable : StaticBody2D
{
	[Export] protected int _pointValue = 1;
	[Export] protected int _maxHealth = 1;
	[Export] protected Texture2D[] _sprites;

	protected int _health;

	protected Sprite2D _sprite;
	protected SessionController refs;

	public override void _Ready()
	{
		_sprite = GetNode("Sprite") as Sprite2D;
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		SetInitialValues();
		AdjustSprite();
	}

	public virtual void Damage(int value)
	{
		_health -= value;

		if (_health <= 0)
		{
			Destroy();
		}

		AdjustSprite();
	}

	protected virtual void SetInitialValues()
	{
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
