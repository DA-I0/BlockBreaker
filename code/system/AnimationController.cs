using Godot;

public partial class AnimationController : Node2D
{
	[Export] private float _minDelay = 0f;
	[Export] private float _maxDelay = 1f;

	public override void _Ready()
	{
		((AnimationPlayer)GetChild(1)).AnimationFinished += RandomizeDelay;
		RandomizeDelay("");
	}

	private void RandomizeDelay(StringName animName)
	{
		float delay = (float)GD.RandRange(_minDelay, _maxDelay);
		((Timer)GetChild(2)).Start(delay);
	}

	private void Trigger()
	{
		((AnimationPlayer)GetChild(1)).Play("idle");
	}
}
