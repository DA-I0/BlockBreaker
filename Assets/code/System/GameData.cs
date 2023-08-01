// TODO: Add dictionary for game localization
public class GameData
{
	public readonly string ConfigFilePath;
	public readonly string ChangelogFilePath;

	public string patchNotes;

	public GameData(string configFilePath, string changelogFilePath)
	{
		ConfigFilePath = configFilePath;
		ChangelogFilePath = changelogFilePath;
	}
}