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

	public string LoadTextResource(string resourcePath)
	{
		return LoadTextFile($"res://assets/text/{resourcePath}");
	}

	public string[] LoadDifficulties(string path)
	{
		if (!DirAccess.DirExistsAbsolute(path))
		{
			return new string[0];
		}

		string[] difficulties = DirAccess.GetFilesAt(path);
		string[] difficultyStrings = new string[difficulties.Length];

		for (int i = 0; i < difficulties.Length; i++)
		{
			difficultyStrings[i] = LoadTextFile($"{path}/{difficulties[i]}");
		}

		return difficultyStrings;
	}

	public void SaveDifficulty(string oldDifficultyName, Difficulty newDifficulty)
	{
		if (oldDifficultyName != string.Empty && oldDifficultyName != newDifficulty.DifficultyName)
		{
			DeleteDifficulty(oldDifficultyName);
		}

		string fileName = $"diff_{newDifficulty.DifficultyName}";

		SaveTextFile(_refs.gameData.CustomDifficultyFolder, fileName, newDifficulty.ToString());
	}

	public void DeleteDifficulty(string difficultyName)
	{
		string filePath = $"{_refs.gameData.CustomDifficultyFolder}/diff_{difficultyName}.txt";

		if (FileAccess.FileExists(filePath))
		{
			DirAccess.RemoveAbsolute(filePath);
		}
	}

	public HighScore[] LoadLeaderboard()
	{
		ConfigFile leaderboardFile = new ConfigFile();
		Error loadingStatus = leaderboardFile.LoadEncryptedPass(_refs.gameData.CustomLeaderboardFilePath, _refs.gameData.EncryptionPassword);

		if (loadingStatus != Error.Ok)
		{
			leaderboardFile.Load(_refs.gameData.DefaultLeaderboardFilePath);
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

		leaderboardFile.SaveEncryptedPass(_refs.gameData.CustomLeaderboardFilePath, _refs.gameData.EncryptionPassword);
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

	private void SaveTextFile(string filePath, string content)
	{
		FindOrCreateDirectory(filePath.Substring(0, filePath.LastIndexOf("/")));

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		file.StoreString(content);
	}

	private void SaveTextFile(string path, string fileName, string content)
	{
		string filePath = $"{path}/{fileName}.txt";
		SaveTextFile(filePath, content);
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
