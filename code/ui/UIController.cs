using Godot;
using System.Collections.Generic;

public partial class UIController : Node
{
	[Export] private Label _gameTitle;
	[Export] private Node _menuButtons;
	private Dictionary<string, CanvasItem> _panels = new Dictionary<string, CanvasItem>();

	private string _activePanel;

	private SessionController refs;

	public event Notification RefreshUI;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;

		FindPanels();
		HideAllPanels();
		TogglePanel("LeaderboardPanel");
		DisplayFirstLanguageSelection();
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

	public void TogglePanel(string panelName)
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
		refs.localization.UpdateUILocalization();
	}

	private void ToggleMenuButtons()
	{
		foreach (CanvasItem button in _menuButtons.GetChildren())
		{
			button.Visible = (_activePanel == "LeaderboardPanel");
		}

		if (((CanvasItem)_menuButtons.GetChild(0)).Visible)
		{
			((Button)_menuButtons.GetChild(0)).GrabFocus();
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

	private void DisplayFirstLanguageSelection()
	{
		(GetChild(1) as Control).Visible = refs.settings.firstLaunch;
	}

	private void ExitGame()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
		GetTree().Quit();
	}
}
