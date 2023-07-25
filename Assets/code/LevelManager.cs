using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	private const int FirstLevelIndex = 1;
	private Dictionary<int, string> _levels = new Dictionary<int, string>();

	public Dictionary<int, string> Levels
	{
		get { return _levels; }
	}

	#region Methods (public)
	public void LoadLevel(int levelIndex)
	{
		SceneManager.LoadScene(levelIndex);
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene("menu");
	}

	public void NextLevel()
	{
		GameObject.Find("_system").GetComponent<GameScore>().AddBonusScore();
		Scene currentScene = SceneManager.GetActiveScene();

		if (currentScene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
		{
			SceneManager.LoadScene(currentScene.buildIndex + 1);
		}
		else
		{
			LoadMainMenu();
		}
	}

	public void Exit()
	{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	#endregion

	#region Methods (private)
	private void Awake()
	{
		FindGameLevels();
	}

	private void FindGameLevels()
	{
		int tempLevelCounter = 1;

		for (int i = FirstLevelIndex; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			_levels.Add(i, "Level " + tempLevelCounter);
			tempLevelCounter++;
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex > 0)
		{
			GameObject.Find("UI").GetComponent<UIGameState>().ToggleGameStats(true);
			GameObject.Find("_system").GetComponent<Gameplay>().SetupLevel();
		}
		else
		{
			GameObject.Find("_system").GetComponent<Gameplay>().Cleanup(true);
			GameObject.Find("UI").GetComponent<UIGameState>().ToggleGameStats(false);
		}
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	#endregion
}
