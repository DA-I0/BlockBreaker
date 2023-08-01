using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
	[SerializeField] private TMP_Text _gameTitle;
	[SerializeField] private TMP_Text _version;

	[SerializeField] private GameObject _levelList;
	[SerializeField] private GameObject _instructionPanel;
	[SerializeField] private GameObject _changelogPanel;
	[SerializeField] private GameObject _optionsPanel;

	private GameObject _activePanel = null;

	private void Start()
	{
		_gameTitle.text = Application.productName;
		_version.text = Application.version;

		HideAllPanels();
	}

	private void HideAllPanels()
	{
		_levelList.SetActive(false);
		_instructionPanel.SetActive(false);
		_changelogPanel.SetActive(false);
		_optionsPanel.SetActive(false);
	}

	public void TogglePanel(string panelName)
	{
		if (_activePanel != null)
		{
			_activePanel.SetActive(false);

			if (_activePanel.name == panelName)
			{
				_activePanel = null;
				return;
			}
		}

		switch (panelName)
		{
			case "level_list":
				_levelList.SetActive(true);
				_levelList.GetComponent<UILevelList>().UpdateLevelList();
				_activePanel = _levelList;
				break;

			case "info_panel":
				_instructionPanel.SetActive(true);
				_activePanel = _instructionPanel;
				break;

			case "changelog_panel":
				_changelogPanel.SetActive(true);
				_activePanel = _changelogPanel;
				BroadcastMessage("UpdateUI");
				break;

			case "options_panel":
				_optionsPanel.SetActive(true);
				_activePanel = _optionsPanel;
				BroadcastMessage("UpdateUI");
				break;

			default:
				break;
		}
	}

	public void ExitGame()
	{
		GetComponent<LevelManager>().Exit();
	}
}
