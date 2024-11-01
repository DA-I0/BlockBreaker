namespace BoGK.Gameplay
{
	public class Skill
	{
		protected int _activationPointsCost;
		protected int _activationPoints;
		protected int _lastUpdateCombo;

		protected GameSystem.SessionController refs;

		public event Notification SkillReady;
		public event Notification SkillUsed;

		public int ActivationCost
		{
			get { return _activationPointsCost; }
		}

		public void Setup(GameSystem.SessionController sessionController)
		{
			refs = sessionController;
			refs.gameScore.ScoreChanged += UpdateActivationPoints;
			refs.levelManager.ResetSession += Cleanup;
			SecondarySetup();
			Reset();
		}

		public virtual void Activate()
		{
			if (refs.paddle.PaddleState != PaddleState.locked && _activationPoints >= _activationPointsCost)
			{
				ApplySkillEffect();
			}
		}

		public override string ToString()
		{
			return base.ToString().Split('.')[^1];
		}

		protected virtual void SecondarySetup() { }
		protected virtual void ApplySkillEffect() { }

		protected void Reset()
		{
			_activationPoints = 0;
			_lastUpdateCombo = 0;
		}

		protected void UpdateActivationPoints(int score, int scoreMultiplier, int combo)
		{
			if (combo == 0)
			{
				_lastUpdateCombo = 0;
				return;
			}

			if (_lastUpdateCombo < combo)
			{
				_lastUpdateCombo = combo;
				_activationPoints++;
			}

			if (_activationPoints >= _activationPointsCost)
			{
				SkillReady?.Invoke();
			}
		}

		protected virtual void OnActivation()
		{
			SkillUsed?.Invoke();
			Reset();
		}

		protected void Cleanup()
		{
			Reset();
			SecondaryCleanup();
			refs.gameScore.ScoreChanged -= UpdateActivationPoints;
			refs.levelManager.ResetSession -= Cleanup;
		}

		protected virtual void SecondaryCleanup() { }
	}
}