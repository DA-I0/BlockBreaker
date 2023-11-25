using Godot;

public partial class UIGameSession : Control
{
	[Export] private Label _score;
	[Export] private Control _skill;
	[Export] private Control _lives;
	[Export] private Control _exitTimer;
	[Export] private Control _exitPrompt;

	private SessionController refs;

	public override void _Ready()
	{
		SetupReferences();
		SetupInitialValues();
	}

	public override void _Process(double delta)
	{
		UpdateTimer();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("game_play") && ((CanvasItem)_exitPrompt).Visible)
		{
			refs.AdvanceCurrentLevel();
		}
	}

	private void SetupReferences()
	{
		refs = GetNode("/root/GameController") as SessionController;
		refs.SkillReady += DisplaySkillIcon;
		refs.SkillUsed += HideSkillIcon;
		refs.gameScore.ScoreChanged += UpdateScore;
		refs.gameScore.TimerStart += DisplayExitTimer;
		refs.gameScore.TimerEnd += DisplayExitPrompt;
		refs.health.LifeChanged += UpdateLives;
		refs.levelManager.ResetSession += HideGameStateUI;
		refs.levelManager.SceneChanged += DisplayGameStateUI;
	}

	private void SetupInitialValues()
	{
		HideExitElements();
		HideSkillIcon();
		UpdateScore(0, 1);
	}

	private void UpdateScore(int score, int scoreMultiplier)
	{
		string multiplier = scoreMultiplier > 1 ? $"(x{scoreMultiplier})" : string.Empty;
		_score.Text = $"{Tr("GAME_SCORE")}: {score} {multiplier}";
	}

	private void UpdateLives(int lives)
	{
		for (int i = 0; i < _lives.GetChildCount(); i++)
		{
			((CanvasItem)_lives.GetChild(i)).Visible = i < (lives - 1);
		}
	}

	private void HideGameStateUI()
	{
		HideExitElements();
		HideSkillIcon();
		Visible = false;
	}

	private void HideExitElements()
	{
		_exitTimer.Visible = false;
		_exitPrompt.Visible = false;
	}

	private void DisplayGameStateUI()
	{
		HideExitElements();
		Visible = true;
	}

	private void DisplayExitTimer()
	{
		_exitTimer.Visible = true;
	}

	private void DisplayExitPrompt()
	{
		_exitTimer.Visible = false;
		_exitPrompt.Visible = true;
	}

	private void UpdateTimer()
	{
		((Label)_exitTimer.GetChild(0)).Text = refs.gameScore.TimeLeft.ToString();
	}

	private void HideSkillIcon()
	{
		_skill.Visible = false;
	}

	private void DisplaySkillIcon()
	{
		_skill.Visible = true;
	}
}
