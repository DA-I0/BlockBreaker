using Godot;

public partial class DamageDealer : Node2D
{
	[Export] protected AnimationPlayer _animator;
	[Export] protected int _strength = 3;

	public override void _Ready()
	{
		_animator.Play("idle");
		_animator.AnimationFinished += Cleanup;
	}

	private void Cleanup(StringName animName)
	{
		_animator.AnimationFinished -= Cleanup;
		QueueFree();
	}

	private void ApplyDamageToObject(Node2D target)
	{
		if (target as Breakable != null)
		{
			((Breakable)(Node)target).Damage(_strength);
			return;
		}
	}
}
