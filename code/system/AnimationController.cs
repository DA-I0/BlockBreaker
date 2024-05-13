using Godot;

namespace BoGK.GameSystem
{
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

			GetNode<AnimationPlayer>("Animator").AnimationFinished += RandomizeDelay;
			RandomizeDelay("");
		}

		private void RandomizeDelay(StringName animName)
		{
			RandomizePosition();
			float delay = (float)GD.RandRange(_minDelay, _maxDelay);
			GetNode<Timer>("Timer").Start(delay);
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
			GetNode<AnimationPlayer>("Animator").Play("idle");
		}
	}
}