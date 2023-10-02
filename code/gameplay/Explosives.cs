using Godot;

public partial class Explosives : Breakable
{
	[Export] protected AnimationPlayer _animator;
	[Export] protected int _strength = 3;

	protected override void SetInitialValues()
	{
		_health = _maxHealth;
		_animator.Play("idle");
	}

	protected override void Destroy()
	{
		_animator.Play("explosion");
		_animator.AnimationFinished += Cleanup;
		refs.gameScore.ChangeScore(_pointValue);
	}

	private void Cleanup(StringName animName)
	{
		if (animName == "explosion")
		{
			_animator.AnimationFinished -= Cleanup;
			QueueFree();
		}
	}

	private void ApplyExplosionToObject(Node2D target)
	{
		if (target as Ball != null)
		{
			((Ball)target).SetTempBoost(_strength / 2);
			return;
		}

		if (target as Breakable != null)
		{
			((Breakable)(Node)target).Damage(_strength);
			return;
		}
	}
}
