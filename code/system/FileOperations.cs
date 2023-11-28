using Godot;

public class FileOperations
{
	SessionController _refs;

	public FileOperations(SessionController refs)
	{
		_refs = refs;
	}

	public string[] GetFileList(string path)
	{
		if (!DirAccess.DirExistsAbsolute(path))
		{
			return new string[0];
		}

		return DirAccess.GetFilesAt(path);
	}

	public Difficulty[] LoadDifficulties(string path)
	{
		if (!DirAccess.DirExistsAbsolute(path))
		{
			return System.Array.Empty<Difficulty>();
		}

		string[] difficultyFiles = DirAccess.GetFilesAt(path);
		System.Collections.Generic.List<Difficulty> difficulties = new System.Collections.Generic.List<Difficulty>();

		foreach (string filePath in difficultyFiles)
		{
			ConfigFile nextDifficulty = new ConfigFile();
			Error error = nextDifficulty.Load($"{path}/{filePath}");

			if (error == Error.Ok)
			{
				difficulties.Add(HelperMethods.DifficultyFromConfig(nextDifficulty));
			}
		}

		return difficulties.ToArray();
	}

	public void SaveDifficulty(string oldDifficultyName, Difficulty newDifficulty)
	{
		if (oldDifficultyName != string.Empty && oldDifficultyName != newDifficulty.DifficultyName)
		{
			DeleteDifficulty(oldDifficultyName);
		}

		string fileName = $"diff_{newDifficulty.DifficultyName}";
		ConfigFile parsedDifficulty = HelperMethods.DifficultyToConfig(newDifficulty);
		parsedDifficulty.Save($"{ProjectSettings.GetSetting("global/CustomDifficultyFolder")}/{fileName}.diff");
	}

	public void DeleteDifficulty(string difficultyName)
	{
		string filePath = $"{ProjectSettings.GetSetting("global/CustomDifficultyFolder")}/diff_{difficultyName}.txt";

		if (FileAccess.FileExists(filePath))
		{
			DirAccess.RemoveAbsolute(filePath);
		}
	}

	public HighScore[] LoadLeaderboard()
	{
		ConfigFile leaderboardFile = new ConfigFile();
		Error loadingStatus = leaderboardFile.LoadEncryptedPass(ProjectSettings.GetSetting("global/CustomLeaderboardFilePath").ToString(), ProjectSettings.GetSetting("global/EncryptionPassword").ToString());

		if (loadingStatus != Error.Ok)
		{
			leaderboardFile.Load(ProjectSettings.GetSetting("global/DefaultLeaderboardFilePath").ToString());
		}

		HighScore[] leaderboard = new HighScore[leaderboardFile.GetSections().Length];
		int index = 0;

		foreach (string player in leaderboardFile.GetSections())
		{
			leaderboard[index].PlayerName = (string)leaderboardFile.GetValue(player, "name");
			leaderboard[index].DifficultyName = (string)leaderboardFile.GetValue(player, "difficulty");
			leaderboard[index].Score = (int)leaderboardFile.GetValue(player, "score");
			index++;
		}

		return leaderboard;
	}

	public void SaveLeaderboard(HighScore[] leaderboard)
	{
		ConfigFile leaderboardFile = new ConfigFile();

		for (int index = 0; index < leaderboard.Length; index++)
		{
			leaderboardFile.SetValue($"Player_{index}", "name", leaderboard[index].PlayerName);
			leaderboardFile.SetValue($"Player_{index}", "difficulty", leaderboard[index].DifficultyName);
			leaderboardFile.SetValue($"Player_{index}", "score", leaderboard[index].Score);
		}

		leaderboardFile.SaveEncryptedPass(ProjectSettings.GetSetting("global/CustomLeaderboardFilePath").ToString(), ProjectSettings.GetSetting("global/EncryptionPassword").ToString());
	}

	public string LoadTextFile(string filePath)
	{
		string fileContent = string.Empty;

		if (FileAccess.FileExists(filePath))
		{
			FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			fileContent = file.GetAsText();
		}

		return fileContent;
	}

	private void FindOrCreateDirectory(string path)
	{
		if (DirAccess.DirExistsAbsolute(path))
		{
			return;
		}

		DirAccess.MakeDirAbsolute(path);
	}
}
