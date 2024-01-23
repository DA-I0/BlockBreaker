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
			if (_exitPrompt.Visible && @event.IsActionReleased("game_play"))
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
				((CanvasItem)_lives.GetChild(i)).Visible = i < lives;
			}
		}

		private void UpdateLifeText(int lives)
		{
			_livesText.Text = $"{Tr("GAME_LIVES")}: {lives}";
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
			UpdateTimerSettings();
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
			_exitPrompt.Visible = true;
		}

		private void UpdateTimer()
		{
			((Label)_exitTimer.GetChild(0)).Text = refs.gameScore.TimeLeft.ToString();
		}

		private void UpdateSkillIcon()
		{
			string activeSkillIconPath = $"{ProjectSettings.GetSetting("global/SkillIconsFilePath")}/skill_{refs.SelectedSkill.GetType()}.png";
			((TextureRect)_skill.GetChild(0)).Texture = ResourceLoader.Load<Texture2D>(activeSkillIconPath);
		}

		private void HideSkillIcon()
		{
			_skill.Visible = false;
		}

		private void DisplaySkillIcon()
		{
			_skill.Visible = true;
		}

		private void UpdateTimerSettings()
		{
			Label timerText = _exitTimer.GetChild<Label>(0);
			Font newTimerFont = ResourceLoader.Load<FontVariation>($"res://assets/fonts/{refs.gameData.TimerFonts[refs.settings.Font].FontName}");

			timerText.AddThemeFontOverride("font", newTimerFont); GD.Print(timerText.HasThemeFontOverride("Label"));
			timerText.AddThemeFontSizeOverride("font_size", refs.gameData.TimerFonts[refs.settings.Font].DefaultSize);
		}
	}
}