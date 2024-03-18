using Godot;

namespace BoGK.UI
{
	public partial class UIOptionsPanel : UIPanel
	{
		[Export] private Label _header;
		[Export] private Control _categoryPanels;
		[Export] private Label _previousCategoryShortuct;
		[Export] private Label _nextCategoryShortuct;
		[Export] private Control[] _categoryFocusTargets;
		[Export] private UIConfirmationPrompt _confirmationPrompt;

		private int _activePanel = 0;

		private UIOptionsBaseCategory ActivePanel
		{
			get { return _categoryPanels.GetChild<UIOptionsBaseCategory>(_activePanel); }
		}

		public GameSystem.SessionController Refs
		{
			get { return refs; }
		}

		public UIController UIController
		{
			get { return uiController; }
		}

		public event Notification FinalizeSetup;

		public override void _Ready()
		{
			SetupBaseReferences();
			FinalizeSetup?.Invoke();
			HideAllPanels();
			DisplayActivePanel();
		}
		public override void _Input(InputEvent @event)
		{
			if (!Visible)
			{
				return;
			}

			if (@event.IsActionPressed("ui_category_prev"))
			{
				SwitchActivePanel(false);
			}

			if (@event.IsActionPressed("ui_category_next"))
			{
				SwitchActivePanel(true);
			}
		}

		protected override void Focus()
		{
			if (Visible && _focusTarget.Length > 0)
			{
				_focusTarget[0]?.GrabFocus();
				ChangeActivePanel(0);
			}
		}

		private void CategoryFocus()
		{
			_categoryFocusTargets[_activePanel]?.GrabFocus();
		}

		private void SaveSettings()
		{
			ActivePanel.ApplySettings();

			refs.settings.SaveSettings();
			UpdateHeaderText();
			refs.localization.UpdateUILocalization();
			_confirmationPrompt.Activate();
		}

		private void CancelSettings()
		{
			refs.settings.LoadSettings();
			ActivePanel.UpdateSettings();
		}

		private void RestoreDefaultSettings()
		{
			ActivePanel.RestoreDefaultSettings();
			ActivePanel.UpdateSettings();
		}

		private void ChangeActivePanel(int targetPanelIndex)
		{
			HideAllPanels();
			_activePanel = targetPanelIndex;
			DisplayActivePanel();
		}

		private void DisplayActivePanel()
		{
			ActivePanel.Enable();
			UpdateHeaderText();
			UpdateShortcutPrompts();
		}

		private void UpdateHeaderText()
		{
			switch (_activePanel)
			{
				case 0:
					_header.Text = $"{Tr("HEADER_OPTIONS")} - {Tr("HEADER_GENERAL")}";
					break;

				case 1:
					_header.Text = $"{Tr("HEADER_OPTIONS")} - {Tr("HEADER_VIDEO")}";
					break;

				case 2:
					_header.Text = $"{Tr("HEADER_OPTIONS")} - {Tr("HEADER_AUDIO")}";
					break;

				case 3:
					_header.Text = $"{Tr("HEADER_OPTIONS")} - {Tr("HEADER_CONTROLS")}";
					break;

				default:
					_header.Text = Tr("HEADER_OPTIONS");
					break;
			}
		}

		private void UpdateShortcutPrompts()
		{
			_previousCategoryShortuct.Text = refs.localization.GetInputSymbol("ui_category_prev", refs.settings.ActiveInputType.ToString())[0];
			_nextCategoryShortuct.Text = refs.localization.GetInputSymbol("ui_category_next", refs.settings.ActiveInputType.ToString())[0];
		}

		private void SwitchActivePanel(bool next)
		{
			_focusIndex = 0;
			int newPanelIndex = next ? ++_activePanel : --_activePanel;

			if (newPanelIndex < 0)
			{
				newPanelIndex = _categoryPanels.GetChildCount() - 1;
			}

			if (newPanelIndex >= _categoryPanels.GetChildCount())
			{
				newPanelIndex = 0;
			}

			ChangeActivePanel(newPanelIndex);
		}

		private void HideAllPanels()
		{
			for (int index = 0; index < _categoryPanels.GetChildCount(); index++)
			{
				_categoryPanels.GetChild<UIOptionsBaseCategory>(index).Disable();
			}
		}
	}
}
