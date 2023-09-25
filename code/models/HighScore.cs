public struct HighScore
{
	public string PlayerName;
	public string DifficultyName;
	public int Score;

	public HighScore(string player, string difficulty, int score)
	{
		PlayerName = player;
		DifficultyName = difficulty;
		Score = score;
	}

	public override string ToString() => $"{PlayerName}:{DifficultyName}:{Score};";
}