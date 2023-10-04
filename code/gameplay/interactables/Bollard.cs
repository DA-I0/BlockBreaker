using Godot;

public partial class Bollard : Interactable
{
	protected override void Toggle()
	{
		if (_isActived)
		{
			_animator.Play("deactivate");
			_animator.Queue("idle_disabled");
		}
		else
		{
			_animator.Play("activate");
			_animator.Queue("idle_enabled");
		}
	}

	private void StartTimer(StringName animationName)
	{
		if (animationName.ToString().Contains("idle_"))
		{
			_timer.Start(_cooldown);
		}
	}
}
