using Godot;

namespace BoGK.UI
{
	public partial class UIStageClearedPanel : PanelContainer
	{
		[Export] private Label _text;

		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.gameScore.StageCleared += DisplayStageSummary;
		}

		private void DisplayStageSummary(int score, int scoreMultiplier, int timeLeft, int enemyClearBonus, int perfectClearBonus)
		{
			if (refs.settings.StageClearScreen && !refs.IsLastLevel)
			{
				_text.Text = (timeLeft > 0) ? $"{Tr("LABEL_TIME_LEFT")}: +{timeLeft}\n" : string.Empty;
				_text.Text += (enemyClearBonus > 0) ? $"{Tr("LABEL_ENEMY_CLEAR")}: +{enemyClearBonus}\n" : string.Empty;
				_text.Text += (perfectClearBonus > 0) ? $"{Tr("LABEL_PERFECT_CLEAR")}: +{perfectClearBonus}\n" : string.Empty;

				_text.Text += (_text.Text != string.Empty && scoreMultiplier > 1) ? $"{Tr("LABEL_SCORE_MULTIPLIER")}: x{scoreMultiplier}\n" : string.Empty;
				_text.Text += (_text.Text == string.Empty) ? $"{Tr("MSG_NO_BONUSES")}\n{Tr("GAME_SCORE")}: {score}" : $"----------------\n{Tr("GAME_SCORE")}: {score}";

				Visible = true;
				refs.ChangeGameState(GameState.stageClear);
			}
			else
			{
				if (timeLeft > 0)
				{
					refs.AdvanceCurrentLevel();
				}
			}
		}
	}
}