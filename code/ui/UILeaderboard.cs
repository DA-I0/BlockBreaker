using Godot;

public partial class UILeaderboard : Panel
{
	[Export] private Label _playerNames;
	[Export] private Label _playerScore;

	private SessionController refs;


	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;
		((UIController)GetNode("../..")).RefreshUI += ApplyLeaderboardValues;
	}

	private void ApplyLeaderboardValues()
	{
		_playerNames.Text = string.Empty;
		_playerScore.Text = string.Empty;

		int position = 1;

		foreach (HighScore entry in refs.gameData.Leaderboard)
		{
			_playerNames.Text += $"{position}. {entry.PlayerName} ({entry.DifficultyName})\n";
			_playerScore.Text += $"{entry.Score}\n";

			position++;
		}
	}
}
