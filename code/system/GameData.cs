using System.Collections.Generic;

public class GameData
{
	public readonly string GameFolderPath;
	public readonly string LevelFolder = "res://scenes/levels";
	public readonly string DefaultDifficultyFolder = "res://assets/text/difficulties";
	public readonly string CustomDifficultyFolder = "user://difficulties";
	public readonly string ChangelogFilePath = "res://assets/text/patchnotes.txt";
	public readonly string DefaultLeaderboardFilePath = "res://assets/text/defaultLeaderboard.txt";
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

	public GameData(SessionController commonRefs, string persistentDataPath, string appDataPath)
	{
		refs = commonRefs;
	}

	public void SetupData()
	{
		LoadLevels();
		LoadDifficulties();
		_patchNotes = refs.fileOperations.LoadTextFile(ChangelogFilePath);
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

	public void LoadLeaderboard()
	{
		string[] entries = refs.fileOperations.LoadLeaderboard().Split(";");

		foreach (string entry in entries)
		{
			string[] values = entry.Split(":");

			if (values.Length < 3)
			{
				continue;
			}

			InsertLeaderboardEntry(new HighScore(values[0].Trim(), values[1].Trim(), int.Parse(values[2])));
		}
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

	// temporary gameplay values
	private static int BlockHealth = 1;
	private static int SturdyBlockHealth = 3;
	private static int SturdyBarrierHealth = 3;
	private static Dictionary<string, string> Controls = new Dictionary<string, string>()
	{
		{"InputKeyboardRelease", "Space"},
		{"InputMouseRelease", "Left Mouse Button"},
		{"InputKeyboardReset", "R"},
		{"InputKeyboardPause", "Escape"}
	};
	// end of temporary gameplay values

	// TODO: Replace manual insertion with an automated system.
	private void InsertGameplayValues(ref string targetString)
	{
		targetString = targetString.Replace('@', ':');
		targetString = targetString.Replace("{input_keyboard_release}", Controls["InputKeyboardRelease"]);
		targetString = targetString.Replace("{input_mouse_release}", Controls["InputMouseRelease"]);
		targetString = targetString.Replace("{input_keyboard_reset}", Controls["InputKeyboardReset"]);
		targetString = targetString.Replace("{input_keyboard_pause}", Controls["InputKeyboardPause"]);
		targetString = targetString.Replace("{block_health}", BlockHealth.ToString());
		targetString = targetString.Replace("{sturdy_block_health}", SturdyBlockHealth.ToString());
		targetString = targetString.Replace("{sturdy_barrier_health}", SturdyBarrierHealth.ToString());
		targetString = targetString.Replace("{combo_multiplier_step}", refs.gameScore.ComboMultiplierStep.ToString());
		targetString = targetString.Replace("{max_combo_multiplier}", refs.gameScore.MaxComboMultiplier.ToString());
		targetString = targetString.Replace("{max_score_multiplier}", refs.gameScore.MaxScoreMultiplier.ToString());
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