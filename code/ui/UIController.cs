using System.Collections.Generic;
using Godot;

public partial class UIController : Node
{
	[Export] private Label _gameTitle;
	[Export] private Label _gameVersion;
	[Export] private Control _menuButtons;
	private Dictionary<string, CanvasItem> _panels = new Dictionary<string, CanvasItem>();

	private string _activePanel = string.Empty;

	private SessionController refs;

	public event Notification RefreshUI;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;

		FindPanels();
		HideAllPanels();
		FocusOnButtons();
		DisplayFirstLanguageSelection();
		_gameTitle.Text = ProjectSettings.GetSetting("application/config/name").ToString();
		_gameVersion.Text = ProjectSettings.GetSetting("global/Version").ToString();
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

		if (panelName != string.Empty && _activePanel != panelName)
		{
			_panels[panelName].Visible = true;
		}

		_activePanel = panelName;

		FocusOnButtons();
		RefreshUI?.Invoke();
		refs.localization.UpdateUILocalization();
	}

	private void FocusOnButtons()
	{
		if (_activePanel == string.Empty)
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
