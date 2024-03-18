namespace BoGK.Gameplay
{
	public class PowerBall : Skill
	{
		private const int PowerPoints = 15;
		private const int SkillDuration = 4;

		private Godot.Timer _skillTimer;

		public PowerBall()
		{
			_activationPointsCost = 15;
		}

		protected override void SecondarySetup()
		{
			_skillTimer = refs.GetChild<Godot.Timer>(refs.GetChildCount() - 1);
			_skillTimer.Timeout += SkillEnd;
		}

		protected override void ApplySkillEffect()
		{
			TogglePowerBallState(true);

			_skillTimer.Start(SkillDuration);
			OnActivation();
			_activationPoints = -999;
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
}