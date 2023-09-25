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
		if (oldDifficultyName != newDifficulty.DifficultyName)
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

	// public string[] ReturnLocalizations()
	// {
	// 	return Directory.GetFiles(_refs.gameData.LocalizationFolder, "*.txt");
	// }

	// public string LoadLocalization(string name)
	// {
	// 	if (!Directory.Exists(_refs.gameData.LocalizationFolder))
	// 	{
	// 		return string.Empty;
	// 	}

	// 	string filePath = $"{_refs.gameData.LocalizationFolder}/{name}.txt";

	// 	return LoadTextFile(filePath);
	// }

	public string LoadLeaderboard()
	{
		string content = LoadTextFile(_refs.gameData.CustomLeaderboardFilePath);

		if (content == string.Empty)
		{
			content = LoadTextFile(_refs.gameData.DefaultLeaderboardFilePath);
		}

		return content;
	}

	public void SaveLeaderboard(HighScore[] leaderboard)
	{
		string content = string.Empty;

		foreach (HighScore entry in leaderboard)
		{
			content += $"{entry}\n";
		}

		string filePath = $"{_refs.gameData.CustomLeaderboardFilePath}.txt";
		SaveTextFile(filePath, content);
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
