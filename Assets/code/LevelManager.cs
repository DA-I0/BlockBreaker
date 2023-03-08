using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	#region Methods (public)
	public void LoadLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene("menu");
	}

	public void NextLevel()
	{
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
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex > 0)
		{
			GameObject.Find("UI").GetComponent<UIGameState>().ToggleGameStats(true);
			GameObject.Find("_system").GetComponent<GameState>().SetupLevel();
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			GameObject.Find("_system").GetComponent<GameState>().Cleanup(true);
			GameObject.Find("UI").GetComponent<UIGameState>().ToggleGameStats(false);
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	#endregion
}
