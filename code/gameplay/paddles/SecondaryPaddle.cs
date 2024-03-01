public partial class SecondaryPaddle : BasePaddle
{
	private DoublePaddle _mainPaddle;

	public override void _Ready()
	{
		_mainPaddle = GetParent<DoublePaddle>();
	}

	public override void ChangeSize(int value)
	{
		_mainPaddle.ChangeSize(value);
	}

	public override void SetPaddleMode(PaddleMode mode)
	{
		_mainPaddle.SetPaddleMode(mode);
	}

	public override void SetPaddleState(PaddleState state, float duration = -1)
	{
		_mainPaddle.SetPaddleState(state, duration);
	}

	public override void ApplyPaddleEffect(Ball targetBall)
	{
		_mainPaddle.ApplyPaddleEffect(targetBall);
	}
}