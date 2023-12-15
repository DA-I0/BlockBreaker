using Godot;

public static class HelperMethods
{
	public static Difficulty DifficultyFromConfig(ConfigFile difficultyConfig)
	{
		Difficulty parsedDifficulty = new Difficulty(
			(string)difficultyConfig.GetValue("", "name"),
			(int)difficultyConfig.GetValue("", "maxLives", 1),
			(int)difficultyConfig.GetValue("", "startingLives", 1),
			(float)difficultyConfig.GetValue("", "ballSpeedMultiplier", 1),
			(float)difficultyConfig.GetValue("", "angleSelectSpeed", 1),
			(int)difficultyConfig.GetValue("", "maxPaddleSize", 1),
			(int)difficultyConfig.GetValue("", "startPaddleSize", 1),
			(int)difficultyConfig.GetValue("", "minPaddleSize", 1),
			(bool)difficultyConfig.GetValue("", "advancingSpeed", false)
			);

		return parsedDifficulty;
	}

	public static ConfigFile DifficultyToConfig(Difficulty difficulty)
	{
		ConfigFile parsedConfig = new ConfigFile();

		parsedConfig.SetValue("", "name", difficulty.DifficultyName);
		parsedConfig.SetValue("", "maxLives", difficulty.MaxLives);
		parsedConfig.SetValue("", "startingLives", difficulty.StartingLives);
		parsedConfig.SetValue("", "ballSpeedMultiplier", difficulty.BallSpeedMultiplier);
		parsedConfig.SetValue("", "angleSelectSpeed", difficulty.AngleSelectSpeed);
		parsedConfig.SetValue("", "maxPaddleSize", difficulty.MaxPaddleSize);
		parsedConfig.SetValue("", "startPaddleSize", difficulty.StartPaddleSize);
		parsedConfig.SetValue("", "minPaddleSize", difficulty.MinPaddleSize);
		parsedConfig.SetValue("", "advancingSpeed", difficulty.AdvancingSpeed);

		return parsedConfig;
	}
}