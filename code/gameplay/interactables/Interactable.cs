using Godot;

public partial class Interactable : Node2D
{
	[Export] protected bool _isActived = true;
	[Export] protected float _cooldown = 1f;

	[Export] protected AnimationPlayer _animator;
	[Export] protected Timer _timer;

	public override void _Ready()
	{
		UpdateState();
	}

	protected virtual void UpdateState()
	{
		string state = _isActived ? "idle_enabled" : "idle_disabled";
		_animator.Play(state);
	}

	protected virtual void Toggle()
	{
		_isActived = !_isActived;
		UpdateState();
	}
}