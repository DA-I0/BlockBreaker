namespace BoGK.Models
{
	public struct HighScore
	{
		public string PlayerName;
		public string DifficultyName;
		public int Score;
		public bool UsedCustomDifficulty;
		public bool UsedCustomSessionLenght;
		public bool UsedStageShuffle;
		public bool UsedDisablePickups;
		public bool UsedDisappearingBall;

		public HighScore(
			string player,
			string difficulty,
			int score,
			bool isCustomDifficulty,
			bool usedCustomSessionLenght,
			bool usedStageShuffle,
			bool usedDisablePickups,
			bool usedDisappearingBall)
		{
			PlayerName = player;
			DifficultyName = difficulty;
			Score = score;
			UsedCustomDifficulty = isCustomDifficulty;
			UsedCustomSessionLenght = usedCustomSessionLenght;
			UsedStageShuffle = usedStageShuffle;
			UsedDisablePickups = usedDisablePickups;
			UsedDisappearingBall = usedDisappearingBall;
		}

		public override string ToString() => $"{PlayerName}:{DifficultyName}:{Score}:{UsedCustomDifficulty};";
	}
}