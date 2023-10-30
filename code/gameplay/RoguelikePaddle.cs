using Godot;

public partial class RoguelikePaddle : Paddle
{
	public override void _Process(double delta)
	{
		UpdateTimeScale();
	}

	private void UpdateTimeScale()
	{
		if (_state == PaddleState.idle && Velocity == Vector2.Zero)
		{
			Engine.TimeScale = 0.1f;
		}
		else
		{
			Engine.TimeScale = 1;
		}
	}
}