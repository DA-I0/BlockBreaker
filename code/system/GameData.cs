using System.Collections.Generic;
using System.Linq;
using BoGK.Models;

public class GameData
{
	public readonly FontVariant[] Fonts = { new FontVariant("Silver_adjusted.tres", 11), new FontVariant("OpenDyslexic-Regular_adjusted.tres", 9) };
	public readonly FontVariant[] TimerFonts = { new FontVariant("Silver_timer.tres", 22), new FontVariant("OpenDyslexic-Regular_timer.tres", 16) };
	private readonly Skill[] _skills = { new BallControl(), new ScreenShake(), new PowerBall(), new EmergencyNet() };

	private List<string> _levels = new List<string>();

	private List<Difficulty> _difficulties = new List<Difficulty>();
	private int _defaultDifficultyCount = 0;

	private string _patchNotes;

	private List<HighScore> _leaderboard = new List<HighScore>();
	private static int MaxLeaderboardEntries = 10;

	private SessionController refs;

	public Skill[] Skills
	{
		get { return _skills; }
	}

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
		LoadPatchNotes();
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

		foreach (string levelFile in FileOperations.GetFileList(Godot.ProjectSettings.GetSetting("global/DefaultLevelFolder").ToString()))
		{
			_levels.Add(levelFile);
		}
	}

	private void LoadDifficulties()
	{
		foreach (Difficulty resourceDifficulty in FileOperations.LoadDifficulties(Godot.ProjectSettings.GetSetting("global/DefaultDifficultyFolder").ToString()))
		{
			AddDifficulty(resourceDifficulty);
		}

		_defaultDifficultyCount = _difficulties.Count;

		foreach (Difficulty resourceDifficulty in FileOperations.LoadDifficulties(Godot.ProjectSettings.GetSetting("global/CustomDifficultyFolder").ToString()))
		{
			AddDifficulty(resourceDifficulty);
		}
	}

	private void LoadPatchNotes()
	{
		string rawPatchNotes = FileOperations.LoadTextFile(Godot.ProjectSettings.GetSetting("global/PatchNotesFilePath").ToString());
		_patchNotes = refs.localization.InsertCustomValues(rawPatchNotes);
	}

	public void LoadLeaderboard()
	{
		_leaderboard = FileOperations.LoadLeaderboard().ToList();
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
		FileOperations.SaveLeaderboard(_leaderboard.ToArray());
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