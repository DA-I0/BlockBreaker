using Godot;

public partial class UILeaderboard : UIPanel
{
	[Export] private Label _playerNames;
	[Export] private Label _playerScore;

	protected override void SetupReferences()
	{
		refs = GetNode("/root/GameController") as SessionController;
		uiController = (UIController)GetNode("../..");
		uiController.RefreshUI += Focus;
		uiController.RefreshUI += ApplyLeaderboardValues;
	}

	private void ApplyLeaderboardValues()
	{
		_playerNames.Text = string.Empty;
		_playerScore.Text = string.Empty;

		int position = 1;

		foreach (HighScore entry in refs.gameData.Leaderboard)
		{
			string difficulty = entry.UsedCustomDifficulty ? $"{entry.DifficultyName}*" : entry.DifficultyName;
			_playerNames.Text += $"{position}. {entry.PlayerName} ({difficulty})\n";
			_playerScore.Text += $"{entry.Score}\n";

			position++;
		}
	}
}
