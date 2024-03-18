using Godot;

namespace BoGK.Gameplay
{
	public partial class Camera : Camera2D
	{
		[Export] private float _maxPositionOffset = 0.5f;
		[Export] private double _shakeDuration = 0.5f;

		private Vector2 _defaultPosition;

		private Timer _timer;
		private GameSystem.SessionController refs;

		public void Shake()
		{
			if (refs.SelectedSkill.GetType() == typeof(ScreenShake) && refs.settings.ScreenShake)
			{
				_timer.Start(_shakeDuration);
			}
		}

		public override void _Ready()
		{
			_defaultPosition = Position;
			_timer = GetChild<Timer>(0);

			refs = GetParent<GameSystem.SessionController>();
			refs.SkillUsed += Shake;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_timer.TimeLeft > 0)
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
}