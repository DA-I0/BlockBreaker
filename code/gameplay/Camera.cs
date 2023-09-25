using Godot;
using System;

public partial class Camera : Camera2D
{
	[Export] private float _maxPositionOffset = 0.5f;
	[Export] private double _shakeDuration = 0.5f;

	private Vector2 _defaultPosition;

	Timer timer;

	public void Shake()
	{
		timer.Start(_shakeDuration);
	}

	public override void _Ready()
	{
		_defaultPosition = Position;
		timer = GetChild(0) as Timer;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("game_play"))
		{
			// Shake();
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if (timer.TimeLeft > 0)
		{
			float horizontalOffset = (float)GD.RandRange(0.0, 1.0) * _maxPositionOffset;
			float verticalOffset = (float)GD.RandRange(0.0, 1.0) * _maxPositionOffset;

			Position = _defaultPosition + new Vector2(horizontalOffset, verticalOffset);
		}
		else
		{
			Position = _defaultPosition;
		}
	}
}
