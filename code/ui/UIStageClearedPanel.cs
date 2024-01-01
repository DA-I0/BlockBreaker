using Godot;

public partial class UIStageClearedPanel : PanelContainer
{
	[Export] private Label _text;

	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;
		refs.gameScore.StageCleared += DisplayStageSummary;
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event.IsActionReleased("game_play"))
		{
			refs.AdvanceCurrentLevel();
		}
	}

	private void DisplayStageSummary(int score, int scoreMultiplier, int timeLeft, int perfectClearBonus)
	{
		_text.Text = (timeLeft > 0) ? $"{Tr("LABEL_TIME_LEFT")}: +{timeLeft}\n" : string.Empty;
		_text.Text += (perfectClearBonus > 0) ? $"{Tr("LABEL_PERFECT_CLEAR")}: +{perfectClearBonus}\n" : string.Empty;

		_text.Text += (_text.Text != string.Empty && scoreMultiplier > 1) ? $"{Tr("LABEL_SCORE_MULTIPLIER")}: x{scoreMultiplier}\n" : string.Empty;
		_text.Text += (_text.Text == string.Empty) ? $"{Tr("MSG_NO_BONUSES")}\n{Tr("GAME_SCORE")}: {score}" : $"----------------\n{Tr("GAME_SCORE")}: {score}";

		Visible = true;
		refs.ChangeGameState(GameState.stageClear);
	}
}
