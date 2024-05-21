namespace BoGK.Gameplay
{
	public class PowerBall : Skill
	{
		private const int SkillDuration = 4;

		private Godot.Timer _skillTimer;

		public PowerBall()
		{
			_activationPointsCost = 15;
		}

		protected override void SecondarySetup()
		{
			_skillTimer = refs.GetNode<Godot.Timer>("SkillTimer");
			_skillTimer.Timeout += SkillEnd;
		}

		protected override void ApplySkillEffect()
		{
			TogglePowerBallState(true);

			_skillTimer.Start(SkillDuration);
			OnActivation();
			_activationPoints = -999;
		}

		protected override void SecondaryCleanup()
		{
			_skillTimer.Timeout -= SkillEnd;
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