public struct Difficulty
{
	public string DifficultyName;
	public int MaxLives;
	public int StartingLives;
	public float BallSpeedMultiplier;
	public float AngleSelectSpeed;
	public int MaxPaddleSize;
	public int StartPaddleSize;
	public int MinPaddleSize;

	public Difficulty(string name, int maxLives, int startingLives, float ballSpeedMultiplier, float angleSelectSpeed, int maxPaddleSize, int startPaddleSize, int minPaddleSize)
	{
		DifficultyName = name;
		MaxLives = maxLives;
		StartingLives = startingLives;
		BallSpeedMultiplier = ballSpeedMultiplier;
		AngleSelectSpeed = angleSelectSpeed;
		MaxPaddleSize = maxPaddleSize;
		StartPaddleSize = startPaddleSize;
		MinPaddleSize = minPaddleSize;
	}

	public override string ToString() => $"name:{DifficultyName};\nmaxLives:{MaxLives};\nstartingLives:{StartingLives};\nballSpeedMultiplier:{BallSpeedMultiplier};\nangleSelectSpeed:{AngleSelectSpeed};\nmaxPaddleSize:{MaxPaddleSize};\nstartPaddleSize:{StartPaddleSize};\nminPaddleSize:{MinPaddleSize};";
}
