using Godot;
using System.Collections.Generic;

public partial class UIController : Node
{
	[Export] private PackedScene _buttonPrefab;

	[Export] private Label _gameTitle;
	[Export] private Node _menuButtons;
	private Dictionary<string, CanvasItem> _panels = new Dictionary<string, CanvasItem>();

	private string _activePanel;

	private SessionController refs;

	public event Notification RefreshUI;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;

		FindPanels();
		PopulateLevelPanel();
		HideAllPanels();
		TogglePanel("LeaderboardPanel");
		_gameTitle.Text = ProjectSettings.GetSetting("application/config/name").ToString();
	}

	private void FindPanels()
	{
		foreach (Node item in GetChild(0).GetChildren())
		{
			if (item.Name.ToString().Contains("Panel"))
			{
				_panels.Add(item.Name, (CanvasItem)item);
			}
		}
	}

	private void TogglePanel(string panelName)
	{
		HideAllPanels();

		if (_activePanel != panelName)
		{
			_panels[panelName].Visible = true;
			_activePanel = panelName;
		}
		else
		{
			TogglePanel("LeaderboardPanel");
		}
		ToggleMenuButtons();
		RefreshUI?.Invoke();
	}

	private void ToggleMenuButtons()
	{
		foreach (CanvasItem button in _menuButtons.GetChildren())
		{
			if (button.Name == "Return")
			{
				button.Visible = (_activePanel != "LeaderboardPanel");
			}
			else
			{
				button.Visible = (_activePanel == "LeaderboardPanel");
			}
		}
	}

	private void SelectLevel(string levelName)
	{
		refs.levelManager.LoadGameScene(levelName);
	}

	private void HideAllPanels()
	{
		foreach (var panel in _panels)
		{
			panel.Value.Visible = false;
		}
	}

	private void PopulateLevelPanel()
	{
		// string[] levels = DirAccess.GetFilesAt("res://scenes/levels");

		// foreach (string level in levels)
		for (int index = 0; index < refs.gameData.Levels.Count; index++)//levels)
		{
			UILevelButton newButton = (UILevelButton)_buttonPrefab.Instantiate();
			newButton.ButtonSetup(index, refs.gameData.Levels[index]);
			_panels["LevelPanel"].AddChild(newButton);
		}
	}

	private void ExitGame()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
		GetTree().Quit();
	}
}
