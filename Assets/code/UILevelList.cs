using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UILevelList : MonoBehaviour
{
	private static string SceneAssetPath;
	private static string SceneFilter = "level*.unity";

	[SerializeField] private GameObject _buttonPrefab;
	private List<string> _levels = new List<string>();

	private bool _levelsLoaded = false;

	public void UpdateLevelList()
	{
		if (_levelsLoaded)
		{
			return;
		}

		foreach (string levelName in _levels)
		{
			Transform levelButton = Instantiate(_buttonPrefab).transform;
			levelButton.SetParent(transform);

			levelButton.GetChild(0).GetComponent<Text>().text = levelName;
			levelButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				GameObject.Find("_system").GetComponent<LevelManager>().LoadLevel(levelName);
			});
		}

		_levelsLoaded = true;
	}

	private void Awake()
	{
		SceneAssetPath = Application.dataPath + "/scenes";
		FindGameLevels();
	}

	private void FindGameLevels()
	{
		string[] sceneFiles = Directory.GetFiles(SceneAssetPath, SceneFilter);

		foreach (string scene in sceneFiles)
		{
			string levelName = scene.Split('/')[^1];
			_levels.Add(levelName.Replace(".unity", string.Empty));
		}
	}
}
