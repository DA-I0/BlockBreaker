using Godot;

public partial class UIGameSession : Node
{
	[Export] private Label _score;
	[Export] private Node _lives;
	[Export] private Node _exitTimer;
	[Export] private Node _exitPrompt;

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
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		refs.gameScore.ScoreChanged += UpdateScore;
		refs.gameScore.TimerStart += DisplayExitTimer;
		refs.gameScore.TimerEnd += DisplayExitPrompt;
		refs.health.LifeChanged += UpdateLives;
		refs.levelManager.ResetSession += HideExitElements;
		refs.levelManager.SceneChanged += HideExitElements;
	}

	private void SetupInitialValues()
	{
		HideExitElements();
		UpdateScore(0, 1);
		UpdateLives(refs.SelectedDifficulty.StartingLives);
	}

	private void UpdateScore(int score, int scoreMultiplier)
	{
		string multiplier = scoreMultiplier > 1 ? $"(x{scoreMultiplier})" : string.Empty;
		_score.Text = $"{Tr("score")}: {score} {multiplier}";
	}

	private void UpdateLives(int lives)
	{
		for (int i = 0; i < _lives.GetChildCount(); i++)
		{
			((CanvasItem)_lives.GetChild(i)).Visible = i < lives;
		}
	}

	private void HideExitElements()
	{
		((CanvasItem)_exitTimer).Visible = false;
		((CanvasItem)_exitPrompt).Visible = false;
	}

	private void DisplayExitTimer()
	{
		((CanvasItem)_exitTimer).Visible = true;
	}

	private void DisplayExitPrompt()
	{
		((CanvasItem)_exitTimer).Visible = false;
		((CanvasItem)_exitPrompt).Visible = true;
	}

	private void UpdateTimer()
	{
		((Label)_exitTimer.GetChild(1)).Text = refs.gameScore.TimeLeft.ToString();
	}
}
