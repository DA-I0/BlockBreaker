using Godot;

namespace BoGK.UI
{
	public partial class UIGameOver : Control
	{
		[Export] private RichTextLabel _header;
		[Export] private Label _score;
		[Export] private LineEdit _name;
		[Export] protected Control _focusTarget;

		private bool canAddToLeaderboard = false;

		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			SetupReferences();
			TogglePrompt();
		}

		private void SetupReferences()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.GameSetup += TogglePrompt;
			refs.levelManager.ResetSession += TogglePrompt;
			refs.GameStateChanged += Display;
		}

		private void Focus()
		{
			if (Visible)
			{
				if (canAddToLeaderboard)
				{
					_name.Clear();
					_name.GrabFocus();
				}
				else
				{
					_focusTarget.GrabFocus();
				}
			}
		}

		private void Display(GameState newState)
		{
			if (newState == GameState.gameOver)
			{
				_header.Text = $"[b][u]{Tr("GAME_OVER")}[/u][/b]";
			}
			else
			{
				_header.Text = $"[b][u]{Tr("GAME_WIN")}[/u][/b]";
			}

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
			Visible = (refs.CurrentGameState == GameState.gameOver || refs.CurrentGameState == GameState.gameWin);
			Focus();
		}

		private void SubmitScore()
		{
			if (canAddToLeaderboard)
			{
				string playerName = (_name.Text == string.Empty) ? Tr(_name.PlaceholderText) : _name.Text;
				string difficultyName = refs.SelectedDifficulty.DifficultyName;

				Models.HighScore playerScore = new Models.HighScore(
					playerName, difficultyName,
					refs.gameScore.CurrentScore,
					refs.IsCustomDifficultySelected,
					(refs.SessionLength > 0),
					refs.ShuffleStages,
					refs.DisablePickups,
					refs.DisappearingBall);

				refs.gameData.AddScoreToLeaderboard(playerScore);
			}

			refs.levelManager.LoadMenuScene();
		}
	}
}