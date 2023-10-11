using Godot;

public partial class AnimationController : Node2D
{
	[Export] private float _minDelay = 0f;
	[Export] private float _maxDelay = 1f;
	[Export] private int _maxOffsetHorizontal = 5;
	[Export] private int _maxOffsetVertical = 5;

	private Vector2 defaultPosition;

	public override void _Ready()
	{
		defaultPosition = Position;

		((AnimationPlayer)GetChild(1)).AnimationFinished += RandomizeDelay;
		RandomizeDelay("");
	}

	private void RandomizeDelay(StringName animName)
	{
		RandomizePosition();
		float delay = (float)GD.RandRange(_minDelay, _maxDelay);
		((Timer)GetChild(2)).Start(delay);
	}

	private void RandomizePosition()
	{
		Vector2 newPosition = defaultPosition;
		newPosition.X += GD.RandRange(-_maxOffsetHorizontal, _maxOffsetHorizontal);
		newPosition.Y += GD.RandRange(-_maxOffsetVertical, _maxOffsetVertical);
		Position = newPosition;
	}

	private void Trigger()
	{
		((AnimationPlayer)GetChild(1)).Play("idle");
	}
}
