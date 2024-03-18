using System.Linq;
using Godot;

namespace BoGK.UI
{
	public partial class UIOptionsControlsPanel : UIOptionsBaseCategory
	{
		[Export] private Control _controlDevices;
		[Export] private Control _keybindChangePanel;
		[Export] private Label _mouseSpeedText;
		[Export] private HSlider _mouseSpeed;
		[Export] private Label _keyboardSpeedText;
		[Export] private HSlider _keyboardSpeed;
		[Export] private Label _joypadSpeedText;
		[Export] private HSlider _joypadSpeed;
		[Export] private OptionButton _activeJoypad;
		[Export] private CheckButton _vibrations;

		private string _inputType = string.Empty;
		private string _inputActionToChange = string.Empty;
		private string _keybindToChange = string.Empty;


		public override void _Ready()
		{
			SetupBaseReferences();
			SetupReferences();
			SetupActiveJoypadOptions();
			_mainPanel.FinalizeSetup += SetupControlCategoryButtons;
			_mainPanel.FinalizeSetup += PopulateKeybindControls;
		}

		public override void _Input(InputEvent @event)
		{
			if (!Visible)
			{
				return;
			}

			if (@event.IsActionPressed("ui_toggle_focus"))
			{
				ToggleFocus();
			}

			if (_inputActionToChange == string.Empty)
			{
				return;
			}

			if (@event is InputEventJoypadButton || @event is InputEventJoypadMotion
				|| @event is InputEventKey || @event is InputEventMouseButton)
			{
				SaveNewKeybinding(@event);
			}
		}

		public override void Disable()
		{
			Visible = false;
			FoldExpandableControls();
		}

		private void SetupReferences()
		{
			Input.JoyConnectionChanged += SetupActiveJoypadOptions;
		}

		public override void UpdateSettings()
		{
			_mouseSpeed.Value = Refs.settings.SpeedMouse;
			_keyboardSpeed.Value = Refs.settings.SpeedKeyboard;
			_joypadSpeed.Value = Refs.settings.SpeedController;
			int activeJoypadIndexHelper = GameSystem.HelperMethods.FindOptionIndex(_activeJoypad, Refs.settings.ActiveController);
			_activeJoypad.Selected = (activeJoypadIndexHelper < 0) ? 0 : activeJoypadIndexHelper;
			_vibrations.ButtonPressed = Refs.settings.ControllerVibrations;

			UpdateKeybindings();
		}

		public override void ApplySettings()
		{
			Refs.settings.SpeedMouse = (float)_mouseSpeed.Value;
			Refs.settings.SpeedKeyboard = (float)_keyboardSpeed.Value;
			Refs.settings.SpeedController = (float)_joypadSpeed.Value;
			Refs.settings.ActiveController = (_activeJoypad.Selected >= 0) ? _activeJoypad.GetItemText(_activeJoypad.Selected) : string.Empty;
			Refs.settings.ActiveControllerID = _activeJoypad.Selected - 1;
			Refs.settings.ControllerVibrations = _vibrations.ButtonPressed;
		}

		public override void RestoreDefaultSettings()
		{
			Refs.settings.SetDefaultControlValues();
		}

		private void UpdateControlsTextValues()
		{
			_mouseSpeedText.Text = $"{System.MathF.Round((float)_mouseSpeed.Value, 2)}x";
			_keyboardSpeedText.Text = $"{System.MathF.Round((float)_keyboardSpeed.Value, 2)}x";
			_joypadSpeedText.Text = $"{System.MathF.Round((float)_joypadSpeed.Value, 2)}x";
		}

		private void SetupActiveJoypadOptions(long device = 0, bool connected = false)
		{
			_activeJoypad.Clear();
			_activeJoypad.AddItem(Tr("LABEL_ALL"), -1);

			foreach (int index in Input.GetConnectedJoypads())
			{
				_activeJoypad.AddItem(Input.GetJoyName(index));
			}
		}

		private void SetupControlCategoryButtons()
		{
			foreach (Control inputType in _controlDevices.GetChildren())
			{
				Button toggleButton = inputType.GetChild<Button>(0);
				Control categoryPanel = inputType.GetChild<Control>(1);
				toggleButton.Pressed += () => ToggleControlCategory(categoryPanel);
				categoryPanel.Visible = false;
			}
		}

		private static void ToggleControlCategory(Control target)
		{
			target.Visible = !target.Visible;

			if (target.Visible)
			{
				target.GetChild(0).GetChild<Control>(1).GrabFocus();
			}
		}

		private void PopulateKeybindControls()
		{
			var inputActions = InputMap.GetActions().Where(a => !a.ToString().StartsWith("ui_"));

			foreach (Control inputType in _controlDevices.GetChildren())
			{
				foreach (var action in inputActions)
				{
					if (inputType.Name.ToString() == "MouseSettings"
						&& (action.ToString().Contains("left") || action.ToString().Contains("right")))
					{
						continue;
					}

					HBoxContainer actionContainer = new HBoxContainer
					{
						Name = action
					};

					Control spacer = new Control
					{
						CustomMinimumSize = new Vector2(8, 0)
					};
					actionContainer.AddChild(spacer);

					Label inputHeader = new Label
					{
						Text = action.ToString().Replace("game_", "option_input_").ToUpper(),
						SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand,
						SizeFlagsStretchRatio = 0.45f
					};
					actionContainer.AddChild(inputHeader);

					actionContainer.AddChild(CreateKeybindButton(inputType.Name.ToString(), action));
					actionContainer.AddChild(CreateKeybindButton(inputType.Name.ToString(), action));

					spacer = new Control
					{
						CustomMinimumSize = new Vector2(1, 0)
					};
					actionContainer.AddChild(spacer);

					inputType.GetChild(1).AddChild(actionContainer);
				}
			}
		}

		private Button CreateKeybindButton(string inputType, string inputAction)
		{
			Button newButton = new Button
			{
				CustomMinimumSize = new Vector2(0, 30),
				SizeFlagsHorizontal = SizeFlags.Fill | SizeFlags.Expand,
				SizeFlagsStretchRatio = 0.25f
			};

			newButton.Pressed += () => ToggleKeybindChange(inputType, inputAction, newButton.Text);
			return newButton;
		}

		private void UpdateKeybindings()
		{
			foreach (Control inputType in _controlDevices.GetChildren())
			{
				if (inputType.Name == "MouseSpeedSetting")
				{
					continue;
				}

				foreach (Control inputAction in inputType.GetChild(1).GetChildren())
				{
					if (!inputAction.Name.ToString().Contains("game_"))
					{
						continue;
					}

					string[] inputEvents = Refs.localization.GetInputSymbol(inputAction.Name, inputAction.GetParent().Name);
					inputAction.GetChild<Button>(2).Text = (inputEvents.Length > 0) ? inputEvents[0] : string.Empty;
					inputAction.GetChild<Button>(3).Text = (inputEvents.Length > 1) ? inputEvents[1] : string.Empty;
				}
			}

			ToggleKeybindChange(string.Empty, string.Empty, string.Empty);
		}

		private void ToggleKeybindChange(string inputType, string inputActionName, string inputValue) // TODO: rename
		{
			_inputActionToChange = inputActionName;
			_keybindToChange = inputValue;
			_keybindChangePanel.Visible = (_inputActionToChange != string.Empty);
		}

		private void SaveNewKeybinding(InputEvent @event)
		{
			if (_inputActionToChange == string.Empty)
			{
				return;
			}

			if (
				(_inputType == "MouseSettings" && @event is not InputEventMouseButton)
				|| (_inputType == "JoypadSettings" && (@event is not InputEventJoypadButton || @event is not InputEventJoypadMotion)
				|| (_inputType == "KeyboardSettings" && (
					@event is InputEventMouseButton || @event is InputEventJoypadButton || @event is InputEventJoypadMotion)
					)
				)
			)
			{
				return;
			}

			Refs.settings.ChangeKeybinding(_inputActionToChange, _keybindToChange, @event);
			UpdateKeybindings();
		}

		private void FoldExpandableControls()
		{
			foreach (Control inputType in _controlDevices.GetChildren())
			{
				inputType.GetChild<Control>(1).Visible = false;
			}
		}
	}
}
