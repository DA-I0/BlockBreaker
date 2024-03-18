using System.Collections.Generic;
using Godot;

namespace BoGK.UI
{
	public partial class UIController : Node
	{
		[Export] private Label _gameTitle;
		[Export] private Label _gameVersion;
		[Export] private Control _menuButtons;
		[Export] private Control _languagePanel;
		private Dictionary<string, CanvasItem> _panels = new Dictionary<string, CanvasItem>();

		private string _activePanel = string.Empty;

		private GameSystem.SessionController refs;

		public event Notification RefreshUI;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.settings.SettingsUpdated += UpdateUIFonts;

			FindPanels();
			HideAllPanels();
			FocusOnButtons();
			DisplayFirstLanguageSelection();
			UpdateUIFonts();
			_gameTitle.Text = ProjectSettings.GetSetting("application/config/name").ToString();
			_gameVersion.Text = ProjectSettings.GetSetting("application/config/version").ToString();
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

		public void FocusOnButtons()
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
			_languagePanel.Visible = refs.settings.FirstLaunch;

			if (_languagePanel.Visible)
			{
				_languagePanel.GetChild<UIFirstLanguageSelection>(0).Focus();
			}
		}

		private void UpdateUIFonts()
		{
			ApplyFontSettings("font_timer_variant", true);
			ApplyFontSettings("font_timer_variant_no_size", false);
		}

		private void ApplyFontSettings(string groupName, bool applySize)
		{
			Font newTimerFont = ResourceLoader.Load<FontVariation>($"res://assets/fonts/{refs.gameData.TimerFonts[refs.settings.Font].FontName}");
			Godot.Collections.Array<Node> nodes = refs.GetTree().GetNodesInGroup(groupName);

			foreach (Node node in nodes)
			{
				((Label)node).AddThemeFontOverride("font", newTimerFont);

				if (applySize)
				{
					((Label)node).AddThemeFontSizeOverride("font_size", refs.gameData.TimerFonts[refs.settings.Font].DefaultSize);
				}
			}
		}

		private void ExitGame()
		{
			GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
			GetTree().Quit();
		}
	}
}