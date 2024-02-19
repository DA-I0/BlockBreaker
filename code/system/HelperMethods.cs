using BoGK.Models;
using Godot;

public static class HelperMethods
{
	public static BreakableVariant VariantFromConfig(ConfigFile config, string section)
	{
		return new BreakableVariant(
			(string)config.GetValue(section, "type_name", section.Replace("Variant - ", string.Empty)),
			(int)config.GetValue(section, "sprite_variant", 0),
			(Color)config.GetValue(section, "custom_color", new Color(1, 1, 1, 1))
			);
	}

	public static ConfigFile VariantsToConfig(ConfigFile config, System.Collections.Generic.Dictionary<string, BreakableVariant> variants)
	{
		ConfigFile updatedConfig = config;

		foreach (BreakableVariant variant in variants.Values)
		{
			updatedConfig.SetValue($"Variant - {variant.TypeName}", "type_name", variant.TypeName);
			updatedConfig.SetValue($"Variant - {variant.TypeName}", "sprite_variant", variant.SpriteVariant);
			updatedConfig.SetValue($"Variant - {variant.TypeName}", "custom_color", variant.CustomColor);
		}

		return updatedConfig;
	}

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
			(bool)difficultyConfig.GetValue("", "advancingSpeed", false),
			(float)difficultyConfig.GetValue("", "pickupSpeedMultiplier", 0.35f)
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
		parsedConfig.SetValue("", "pickupSpeedMultiplier", difficulty.PickupSpeedMultiplier);

		return parsedConfig;
	}

	public static bool IsActiveInputDevice(SessionController refs, InputEvent @event)
	{
		if (refs.settings.ActiveControllerID > -1 && @event.AsText().Contains("Joypad"))
		{
			return @event.Device == refs.settings.ActiveControllerID;
		}

		return true;
	}
}