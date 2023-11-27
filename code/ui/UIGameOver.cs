using Godot;

public partial class UIGameOver : Control
{
	[Export] private RichTextLabel _header;
	[Export] private Label _score;
	[Export] private LineEdit _name;

	private bool canAddToLeaderboard = false;

	private SessionController refs;

	public override void _Ready()
	{
		SetupReferences();
		TogglePrompt();
	}

	private void SetupReferences()
	{
		refs = GetNode("/root/GameController") as SessionController;
		refs.GameSetup += TogglePrompt;
		refs.levelManager.ResetSession += TogglePrompt;
		refs.GameStateChanged += DisplayVictoryPrompt;
		refs.health.GameOver += DisplayGameOverPrompt;
	}

	private void DisplayVictoryPrompt()
	{
		if (refs.CurrentGameState == GameState.gameOver)
		{
			_header.Text = $"[b][u]{Tr("GAME_WIN")}[/u][/b]";
			SetupScoreValues();
		}
	}

	private void DisplayGameOverPrompt()
	{
		refs.ChangeGameState(GameState.gameOver);
		_header.Text = $"[b][u]{Tr("GAME_OVER")}[/u][/b]";
		SetupScoreValues();
	}

	private void SetupScoreValues()
	{
		canAddToLeaderboard = refs.gameData.CanScoreJoinLeaderboard(refs.gameScore.CurrentScore);
		_score.Text = $"{Tr("GAME_SCORE")}: {refs.gameScore.CurrentScore}";
		_name.Visible = canAddToLeaderboard;

		TogglePrompt();
	}

	private void TogglePrompt()
	{
		Visible = (refs.CurrentGameState == GameState.gameOver);
	}

	private void SubmitScore()
	{
		if (canAddToLeaderboard)
		{
			string playerName = (_name.Text == string.Empty) ? Tr(_name.PlaceholderText) : _name.Text;

			string difficultyName = refs.SelectedDifficulty.DifficultyName;
			difficultyName += refs.IsCustomDifficultySelected ? "*" : string.Empty;

			HighScore playerScore = new HighScore(playerName, difficultyName, refs.gameScore.CurrentScore);
			refs.gameData.AddScoreToLeaderboard(playerScore);
		}

		refs.levelManager.LoadMenuScene();
	}
}
