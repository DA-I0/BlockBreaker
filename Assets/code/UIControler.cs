using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
	[SerializeField] private Text _gameTitle;
	[SerializeField] private Text _version;

	[SerializeField] private GameObject _levelList;

	private void Start()
	{
		_gameTitle.text = Application.productName;
		_version.text = Application.version;

		_levelList.SetActive(false);
	}

	public void ToggleLevelList()
	{
		_levelList.SetActive(!_levelList.activeSelf);
		_levelList.GetComponent<UILevelList>().UpdateLevelList();
	}

	public void ExitGame()
	{
		GetComponent<LevelManager>().Exit();
	}
}
