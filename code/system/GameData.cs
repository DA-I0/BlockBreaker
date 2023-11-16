using System.Collections.Generic;
using System.Linq;

public class GameData
{
	public readonly string EncryptionPassword = "Gopher's are totally fake you guys!";
	public readonly string GameFolderPath;
	public readonly string LevelFolder = "res://scenes/levels";
	public readonly string DefaultDifficultyFolder = "res://assets/text/difficulties";
	public readonly string CustomDifficultyFolder = "user://difficulties";
	public readonly string ConfigFilePath = "user://settings.cfg";
	public readonly string ChangelogFilePath = "res://assets/text/patchnotes.txt";
	public readonly string DefaultLeaderboardFilePath = "res://assets/text/default_leaderboard.txt";
	public readonly string CustomLeaderboardFilePath = "user://leaderboard.txt";

	private List<string> _levels = new List<string>();

	private List<Difficulty> _difficulties = new List<Difficulty>();
	private int _defaultDifficultyCount = 0;

	private string _changelog;

	private List<HighScore> _leaderboard = new List<HighScore>();
	private static int MaxLeaderboardEntries = 10;

	private SessionController refs;

	public List<string> Levels
	{
		get { return _levels; }
	}

	public List<Difficulty> Difficulties
	{
		get { return _difficulties; }
	}

	public int DefaultDifficultyCount
	{
		get { return _defaultDifficultyCount; }
	}

	public string Changelog
	{
		get { return _changelog; }
	}

	public List<HighScore> Leaderboard
	{
		get { return _leaderboard; }
	}

	public GameData(SessionController commonRefs)
	{
		refs = commonRefs;
	}

	public void SetupData()
	{
		LoadLevels();
		LoadDifficulties();
		LoadChangelog();
		LoadLeaderboard();
	}

	public void AddDifficulty(Difficulty newDifficulty)
	{
		_difficulties.Add(newDifficulty);
	}

	public void AddDifficulty(string difficultyString)
	{
		Difficulty newDifficulty = ParseDifficulty(difficultyString);
		AddDifficulty(newDifficulty);
	}

	public void UpdateDifficulty(int index, Difficulty newDifficulty)
	{
		_difficulties[index] = newDifficulty;
	}

	public void RemoveDifficulty(int index)
	{
		_difficulties.RemoveAt(index);
	}

	private void LoadLevels()
	{
		_levels.Clear();

		foreach (string levelFile in refs.fileOperations.GetFileList(LevelFolder))
		{
			_levels.Add(levelFile);
		}
	}

	private void LoadDifficulties()
	{
		foreach (string resourceDifficulty in refs.fileOperations.LoadDifficulties(DefaultDifficultyFolder))
		{
			AddDifficulty(resourceDifficulty);
		}

		_defaultDifficultyCount = _difficulties.Count;

		foreach (string resourceDifficulty in refs.fileOperations.LoadDifficulties(CustomDifficultyFolder))
		{
			AddDifficulty(resourceDifficulty);
		}
	}

	private void LoadChangelog()
	{
		_changelog = refs.fileOperations.LoadTextFile(ChangelogFilePath);
	}

	public void LoadLeaderboard()
	{
		_leaderboard = refs.fileOperations.LoadLeaderboard().ToList();
	}

	public bool CanScoreJoinLeaderboard(int newScore)
	{
		for (int i = 0; i < _leaderboard.Count; i++)
		{
			if (i >= _leaderboard.Count || newScore > _leaderboard[i].Score)
			{
				return true;
			}
		}

		return false;
	}

	public void AddScoreToLeaderboard(HighScore newEntry)
	{
		InsertLeaderboardEntry(newEntry);
		refs.fileOperations.SaveLeaderboard(_leaderboard.ToArray());
	}

	private Difficulty ParseDifficulty(string difficultyString)
	{
		string[] difficultySettings = difficultyString.Split(";");

		Difficulty parsedDifficulty = new Difficulty(
			difficultySettings[0].Split(":")[1],
			int.Parse(difficultySettings[1].Split(":")[1]),
			int.Parse(difficultySettings[2].Split(":")[1]),
			float.Parse(difficultySettings[3].Split(":")[1]),
			float.Parse(difficultySettings[4].Split(":")[1]),
			int.Parse(difficultySettings[5].Split(":")[1]),
			int.Parse(difficultySettings[6].Split(":")[1]),
			int.Parse(difficultySettings[7].Split(":")[1])
			);

		return parsedDifficulty;
	}

	private void InsertLeaderboardEntry(HighScore newEntry)
	{
		_leaderboard.Add(newEntry);
		_leaderboard.Sort((entry1, entry2) => entry1.Score.CompareTo(entry2.Score));
		_leaderboard.Reverse();
		PruneLeaderboard();
	}

	private void PruneLeaderboard()
	{
		while (_leaderboard.Count > MaxLeaderboardEntries)
		{
			_leaderboard.RemoveAt(MaxLeaderboardEntries);
		}
	}
}