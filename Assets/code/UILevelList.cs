using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelList : MonoBehaviour
{
	[SerializeField] private GameObject _buttonPrefab;

	private LevelManager levelManager;
	private bool _levelsLoaded = false;

	public void UpdateLevelList()
	{
		if (_levelsLoaded)
		{
			return;
		}

		foreach (var level in levelManager.Levels)
		{
			Transform levelButton = Instantiate(_buttonPrefab).transform;
			levelButton.SetParent(transform);

			levelButton.GetChild(0).GetComponent<TMP_Text>().text = level.Value;
			levelButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				levelManager.LoadLevel(level.Key);
			});
		}

		_levelsLoaded = true;
	}

	private void Awake()
	{
		levelManager = GameObject.Find("_system").GetComponent<LevelManager>();
	}
}
