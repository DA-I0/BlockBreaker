using Godot;

namespace BoGK.UI
{
	public partial class UIGameSession : Control
	{
		[Export] private Label _score;
		[Export] private Control _skill;
		[Export] private Control _lives;
		[Export] private Label _livesText;
		[Export] private Control _exitTimer;
		[Export] private Control _exitPrompt;
		[Export] private Control _stageClearPrompt;

		private GameSystem.SessionController refs;

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
			if (_exitPrompt.Visible && @event.IsActionReleased("game_play"))
			{
				if (!refs.settings.StageClearScreen)
				{
					refs.gameScore.AddBonusScore();
					refs.AdvanceCurrentLevel();
					return;
				}

				if (!_stageClearPrompt.Visible)
				{
					refs.gameScore.AddBonusScore();
				}
				else
				{
					refs.AdvanceCurrentLevel();
				}
			}
		}

		private void SetupReferences()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.SkillReady += DisplaySkillIcon;
			refs.SkillUsed += HideSkillIcon;
			refs.GameStateChanged += ActOnGameState;
			refs.gameScore.ScoreChanged += UpdateScore;
			refs.gameScore.StageCleared += DisplayExitPrompt;
			refs.gameScore.TimerStart += DisplayExitTimer;
			refs.gameScore.ExitTimer.Timeout += DisplayExitPrompt;
			refs.health.LifeChanged += UpdateLives;
			refs.levelManager.ResetSession += HideGameStateUI;
			refs.levelManager.SceneChanged += DisplayGameStateUI;
		}

		private void SetupInitialValues()
		{
			HideExitElements();
			HideSkillIcon();
			UpdateScore(0, 1, 0);
		}

		private void UpdateScore(int score, int scoreMultiplier, int combo)
		{
			string multiplier = scoreMultiplier > 1 ? $"(x{scoreMultiplier})" : string.Empty;
			_score.Text = $"{Tr("GAME_SCORE")}: {score} {multiplier}";
		}

		private void UpdateLives(int lives)
		{
			UpdatedLifeIcons(lives);
			UpdateLifeText(lives);
		}

		private void UpdatedLifeIcons(int lives)
		{
			for (int i = 0; i < _lives.GetChildCount(); i++)
			{
				_lives.GetChild<CanvasItem>(i).Visible = i < (lives - 1);
			}
		}

		private void UpdateLifeText(int lives)
		{
			int remainingLives = lives > 0 ? lives - 1 : 0;
			_livesText.Text = $"{Tr("GAME_LIVES")}: {remainingLives}";
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
			_stageClearPrompt.Visible = false;
		}

		private void DisplayGameStateUI()
		{
			HideExitElements();
			DisplayLifeElements();
			UpdateSkillIcon();
			Visible = true;
		}

		private void DisplayLifeElements()
		{
			_lives.Visible = !refs.settings.LivesAsText;
			_livesText.Visible = refs.settings.LivesAsText;
		}

		private void DisplayExitTimer()
		{
			_exitTimer.Visible = true;
		}

		private void DisplayExitPrompt()
		{
			_exitTimer.Visible = false;

			if (!refs.IsLastLevel)
			{
				_exitPrompt.Visible = true;
			}
		}

		private void DisplayExitPrompt(int score, int scoreMultiplier, int timeLeft, int enemyClearBonus, int perfectClearBonus)
		{
			DisplayExitPrompt();
		}

		private void UpdateTimer()
		{
			_exitTimer.GetNode<Label>("TimeLeft").Text = refs.gameScore.TimeLeft.ToString();
		}

		private void UpdateSkillIcon()
		{
			string activeSkillIconPath = $"{ProjectSettings.GetSetting("global/SkillIconsFilePath")}/skill_{refs.SelectedSkill.ToString()}.png";
			_skill.GetNode<TextureRect>("SkillIcon").Texture = ResourceLoader.Load<Texture2D>(activeSkillIconPath);
		}

		private void HideSkillIcon()
		{
			_skill.Visible = false;
		}

		private void DisplaySkillIcon()
		{
			_skill.Visible = true;
		}

		private void ActOnGameState(GameState newGameState)
		{
			if (newGameState == GameState.gameOver || newGameState == GameState.gameWin)
			{
				GD.Print("hide panels on game end");
				HideExitElements();
			}
		}
	}
}