public class BallControl : Skill
{
	public BallControl()
	{
		_activationPointsCost = 5;
	}

	public override void Activate()
	{
		if (refs.paddle.PaddleState != PaddleState.locked && _activationPoints >= _activationPointsCost)
		{
			for (int i = 0; i < refs.Balls.Count; i++)
			{
				((Ball)refs.Balls[i]).BallMode = BallMode.angleSelection;
			}

			OnActivation();
		}
	}
}