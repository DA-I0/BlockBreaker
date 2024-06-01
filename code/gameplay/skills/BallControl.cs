namespace BoGK.Gameplay
{
	public class BallControl : Skill
	{
		public BallControl()
		{
			_activationPointsCost = 5;
		}

		protected override void ApplySkillEffect()
		{
			for (int i = 0; i < refs.Balls.Count; i++)
			{
				((Ball)refs.Balls[i]).BallMode = BallMode.angleSelection;
			}

			OnActivation();
		}
	}
}