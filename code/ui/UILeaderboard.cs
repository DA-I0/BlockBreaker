using Godot;

namespace BoGK.UI
{
	public partial class UILeaderboard : UIPanel
	{
		[Export] private Label _playerNames;
		[Export] private Label _playerScore;

		public override void _Ready()
		{
			SetupBaseReferences();
			SetupReferences();
		}

		protected void SetupReferences()
		{
			uiController.RefreshUI += ApplyLeaderboardValues;
		}

		private void ApplyLeaderboardValues()
		{
			_playerNames.Text = string.Empty;
			_playerScore.Text = string.Empty;

			int position = 1;

			foreach (Models.HighScore entry in refs.gameData.Leaderboard)
			{
				string difficulty = entry.UsedCustomDifficulty ? $"{entry.DifficultyName}*" : Tr(entry.DifficultyName);
				_playerNames.Text += $"{position}. {entry.PlayerName} ({difficulty})\n";
				_playerScore.Text += $"{entry.Score}\n";

				position++;
			}
		}
	}
}