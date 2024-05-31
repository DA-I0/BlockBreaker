using BoGK.Models;
using Godot;

namespace BoGK.GameSystem
{
	public static class FileOperations
	{
		public static string[] GetFolderList(string path)
		{
			if (!DirAccess.DirExistsAbsolute(path))
			{
				return System.Array.Empty<string>();
			}

			return DirAccess.GetDirectoriesAt(path);
		}

		public static string[] GetFileList(string path)
		{
			if (!DirAccess.DirExistsAbsolute(path))
			{
				return System.Array.Empty<string>();
			}

			return DirAccess.GetFilesAt(path);
		}

		public static Difficulty[] LoadDifficulties(string path)
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

		public static void SaveDifficulty(string oldDifficultyName, Difficulty newDifficulty)
		{
			if (oldDifficultyName != string.Empty && oldDifficultyName != newDifficulty.DifficultyName)
			{
				DeleteDifficulty(oldDifficultyName);
			}

			FindOrCreateDirectory(ProjectSettings.GetSetting("global/CustomDifficultyFolder").ToString());

			string fileName = $"diff_{newDifficulty.DifficultyName}";
			ConfigFile parsedDifficulty = HelperMethods.DifficultyToConfig(newDifficulty);
			parsedDifficulty.Save($"{ProjectSettings.GetSetting("global/CustomDifficultyFolder")}/{fileName}.diff");
		}

		public static void DeleteDifficulty(string difficultyName)
		{
			string filePath = $"{ProjectSettings.GetSetting("global/CustomDifficultyFolder")}/diff_{difficultyName}.diff";

			if (FileAccess.FileExists(filePath))
			{
				DirAccess.RemoveAbsolute(filePath);
			}
		}

		public static HighScore[] LoadLeaderboard()
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
				leaderboard[index].UsedCustomDifficulty = (bool)leaderboardFile.GetValue(player, "usedCustomDifficulty");
				leaderboard[index].UsedCustomSessionLenght = (bool)leaderboardFile.GetValue(player, "usedCustomSessionLenght", false);
				leaderboard[index].UsedStageShuffle = (bool)leaderboardFile.GetValue(player, "usedStageShuffle", false);
				leaderboard[index].UsedDisablePickups = (bool)leaderboardFile.GetValue(player, "usedDisablePickups", false);
				leaderboard[index].UsedDisappearingBall = (bool)leaderboardFile.GetValue(player, "usedCustomDifficulty", false);
				index++;
			}

			return leaderboard;
		}

		public static void SaveLeaderboard(HighScore[] leaderboard)
		{
			ConfigFile leaderboardFile = new ConfigFile();

			for (int index = 0; index < leaderboard.Length; index++)
			{
				leaderboardFile.SetValue($"Player_{index}", "name", leaderboard[index].PlayerName);
				leaderboardFile.SetValue($"Player_{index}", "difficulty", leaderboard[index].DifficultyName);
				leaderboardFile.SetValue($"Player_{index}", "score", leaderboard[index].Score);
				leaderboardFile.SetValue($"Player_{index}", "usedCustomDifficulty", leaderboard[index].UsedCustomDifficulty);
				leaderboardFile.SetValue($"Player_{index}", "usedCustomSessionLenght", leaderboard[index].UsedCustomSessionLenght);
				leaderboardFile.SetValue($"Player_{index}", "usedStageShuffle", leaderboard[index].UsedStageShuffle);
				leaderboardFile.SetValue($"Player_{index}", "usedDisablePickups", leaderboard[index].UsedDisablePickups);
				leaderboardFile.SetValue($"Player_{index}", "usedDisappearingBall", leaderboard[index].UsedDisappearingBall);
			}

			leaderboardFile.SaveEncryptedPass(ProjectSettings.GetSetting("global/CustomLeaderboardFilePath").ToString(), ProjectSettings.GetSetting("global/EncryptionPassword").ToString());
		}

		public static string LoadTextFile(string filePath)
		{
			string fileContent = string.Empty;

			if (FileAccess.FileExists(filePath))
			{
				FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
				fileContent = file.GetAsText();
			}

			return fileContent;
		}

		private static void FindOrCreateDirectory(string path)
		{
			if (DirAccess.DirExistsAbsolute(path))
			{
				return;
			}

			DirAccess.MakeDirAbsolute(path);
		}
	}
}