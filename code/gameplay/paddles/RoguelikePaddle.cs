using Godot;

namespace BoGK.Gameplay
{
	public partial class RoguelikePaddle : Paddle
	{
		[Export] private double _minTimeScale = 0.1;
		[Export] private double _slowdown = 0.05;

		public override void _Process(double delta)
		{
			UpdateTimeScale();
		}

		private void UpdateTimeScale()
		{
			if (_state == PaddleState.idle && Velocity == Vector2.Zero)
			{
				float newTimeScale = (float)Mathf.Lerp(Engine.TimeScale, _minTimeScale, _slowdown);
				Engine.TimeScale = newTimeScale;
			}
			else
			{
				Engine.TimeScale = 1;
			}
		}
	}
}