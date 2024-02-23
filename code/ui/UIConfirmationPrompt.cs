using Godot;

public partial class UIConfirmationPrompt : Control
{
	[Export] private AnimationPlayer _animator;

	public void Activate()
	{
		_animator.Play("activate");
	}
}