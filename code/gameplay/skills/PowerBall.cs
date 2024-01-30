public class PowerBall : Skill
{
	private const int PowerPoints = 15;
	private const int SkillDuration = 5;

	private Godot.Timer _skillTimer;

	public PowerBall()
	{
		_activationPointsCost = 15;
	}

	public override void Activate()
	{
		if (refs.paddle.PaddleState != PaddleState.locked && _activationPoints >= _activationPointsCost)
		{
			TogglePowerBallState(true);

			_skillTimer.Start(SkillDuration);
			OnActivation();
			_activationPoints = -999;
		}
	}

	protected override void SecondarySetup()
	{
		_skillTimer = refs.GetChild<Godot.Timer>(refs.GetChildCount() - 1);
		_skillTimer.Timeout += SkillEnd;
	}

	private void TogglePowerBallState(bool activate)
	{
		for (int i = 0; i < refs.Balls.Count; i++)
		{
			((Ball)refs.Balls[i]).SetPowerBallState(activate);
		}
	}

	private void SkillEnd()
	{
		TogglePowerBallState(false);
		Reset();
	}
}