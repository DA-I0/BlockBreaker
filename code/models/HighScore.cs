public struct HighScore
{
	public string PlayerName;
	public string DifficultyName;
	public int Score;
	public bool UsedCustomDifficulty;

	public HighScore(string player, string difficulty, int score, bool isCustomDifficulty)
	{
		PlayerName = player;
		DifficultyName = difficulty;
		Score = score;
		UsedCustomDifficulty = isCustomDifficulty;
	}

	public override string ToString() => $"{PlayerName}:{DifficultyName}:{Score}:{UsedCustomDifficulty};";
}