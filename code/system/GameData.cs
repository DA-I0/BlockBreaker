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

	private string _patchNotes;

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

	public string PatchNotes
	{
		get { return _patchNotes; }
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
		foreach (Difficulty resourceDifficulty in refs.fileOperations.LoadDifficulties(DefaultDifficultyFolder))
		{
			AddDifficulty(resourceDifficulty);
		}

		_defaultDifficultyCount = _difficulties.Count;

		foreach (Difficulty resourceDifficulty in refs.fileOperations.LoadDifficulties(CustomDifficultyFolder))
		{
			AddDifficulty(resourceDifficulty);
		}
	}

	private void LoadChangelog()
	{
		string rawPatchNotes = refs.fileOperations.LoadTextFile(ChangelogFilePath);
		_patchNotes = refs.localization.InsertCustomValues(rawPatchNotes);
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