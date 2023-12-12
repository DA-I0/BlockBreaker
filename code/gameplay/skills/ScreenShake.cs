using Godot;

public partial class ScreenShake : ISkill
{
	private int _activationPointsCost = 10;
	private int _debrisCount = 5;
	private string _prefabPath = "res://prefabs/skills/screenshake_debris.tscn";
	private int _maxOffesetHor = 95;
	private int _maxOffesetVer = 60;

	private int _activationPoints;
	private int _lastUpdateCombo;

	private SessionController refs;

	public event Notification SkillReady;
	public event Notification SkillUsed;

	public void Setup(SessionController sessionController)
	{
		refs = sessionController;
		refs.gameScore.ScoreChanged += UpdateActivationPoints;
		refs.levelManager.ResetSession += Cleanup;
		Reset();
	}

	public void Activate()
	{
		if (refs.paddle.PaddleState != PaddleState.locked && _activationPoints >= _activationPointsCost)
		{
			refs.paddle.SetPaddleState(PaddleState.confused, 3f);

			for (int i = 0; i < refs.Balls.Count; i++)
			{
				float angleChange = GD.RandRange(5, 15);
				int direction = GD.RandRange(-1, 1) < 0 ? -1 : 1;
				((Ball)refs.Balls[i]).ChangeRotation(angleChange * direction);
			}

			Node level = refs.GetNode("CurrentScene").GetChild(0);

			for (int index = 0; index < _debrisCount; index++)
			{
				Node2D debris = (Node2D)ResourceLoader.Load<PackedScene>(_prefabPath).Instantiate();
				debris.Position = new Vector2(GD.RandRange(-_maxOffesetHor, _maxOffesetHor), GD.RandRange(-_maxOffesetVer, _maxOffesetVer));
				level.AddChild(debris);
			}

			refs.audioController.PlayAudio(3);
			SkillUsed?.Invoke();
			Reset();
		}
	}

	private void Reset()
	{
		_activationPoints = 0;
		_lastUpdateCombo = 0;
	}

	private void UpdateActivationPoints(int score, int scoreMultiplier, int combo)
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

	private void Cleanup()
	{
		Reset();
		refs.gameScore.ScoreChanged -= UpdateActivationPoints;
		refs.levelManager.ResetSession -= Cleanup;
	}
}