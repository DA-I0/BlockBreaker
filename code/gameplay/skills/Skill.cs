public class Skill
{
	protected int _activationPointsCost;
	protected int _activationPoints;
	protected int _lastUpdateCombo;

	protected SessionController refs;

	public event Notification SkillReady;
	public event Notification SkillUsed;

	public void Setup(SessionController sessionController)
	{
		refs = sessionController;
		refs.gameScore.ScoreChanged += UpdateActivationPoints;
		refs.levelManager.ResetSession += Cleanup;
		Reset();
	}

	public virtual void Activate() { }


	protected void Reset()
	{
		_activationPoints = 0;
		_lastUpdateCombo = 0;
	}

	protected void UpdateActivationPoints(int score, int scoreMultiplier, int combo)
	{
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
		refs.gameScore.ScoreChanged -= UpdateActivationPoints;
		refs.levelManager.ResetSession -= Cleanup;
	}
}