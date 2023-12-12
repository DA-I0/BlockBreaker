public class BallTurner : Skill
{
	public BallTurner()
	{
		_activationPointsCost = 10;
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