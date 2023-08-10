using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelList : MonoBehaviour
{
	[SerializeField] private GameObject _buttonPrefab;

	private LevelManager _levelManager;
	private bool _levelsLoaded = false;

	public void UpdateLevelList()
	{
		if (_levelsLoaded)
		{
			return;
		}

		foreach (var level in _levelManager.Levels)
		{
			Transform levelButton = Instantiate(_buttonPrefab).transform;
			levelButton.SetParent(transform);
			levelButton.localScale = new Vector3(1, 1, 1);
			levelButton.GetChild(0).GetComponent<TMP_Text>().text = level.Value;
			levelButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				_levelManager.LoadLevel(level.Key);
			});
		}

		_levelsLoaded = true;
	}

	private void Awake()
	{
		_levelManager = GameObject.Find("_system").GetComponent<LevelManager>();
	}
}
