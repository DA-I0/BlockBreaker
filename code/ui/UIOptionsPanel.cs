using System.Collections.Generic;
using System.Linq;
using BoGK.Models;
using Godot;

namespace BoGK.UI // TODO: break each category into a separate scene
{
	public partial class UIOptionsPanel : UIPanel
	{
		private const int StaticItems = 5;
		[Export] private Label _header;
		[Export] private OptionButton _language;
		[Export] private OptionButton _font;
		[Export] private OptionButton _controllerType;
		[Export] private CheckButton _livesDisplay;
		[Export] private CheckButton _stageClearDisplay;
		[Export] private Control _controlDevices;
		[Export] private CheckButton _fullscreen;
		[Export] private CheckButton _screenShake;
		[Export] private OptionButton _pickupOrder;
		[Export] private Label _backgroundBrightnessText;
		[Export] private HSlider _backgroundBrightness;
		[Export] private Label _helperTransparencyText;
		[Export] private HSlider _helperTransparency;
		[Export] private Control _brekableVariantContainer;
		[Export] private UIColorPicker _colorPickerPanel;
		[Export] private Label _masterVolumeText;
		[Export] private HSlider _masterVolume;
		[Export] private Label _musicVolumeText;
		[Export] private HSlider _musicVolume;
		[Export] private Label _effectsVolumeText;
		[Export] private HSlider _effectsVolume;
		[Export] private Control _keybindChangePanel;
		[Export] private Label _mouseSpeedText;
		[Export] private HSlider _mouseSpeed;
		[Export] private Label _keyboardSpeedText;
		[Export] private HSlider _keyboardSpeed;
		[Export] private Label _joypadSpeedText;
		[Export] private HSlider _joypadSpeed;
		[Export] private OptionButton _activeJoypad;
		[Export] private CheckButton _vibrations;
		[Export] private Label _previousCategoryShortuct;
		[Export] private Label _nextCategoryShortuct;
		[Export] private Control[] _categoryFocusTargets;
		[Export] private UIConfirmationPrompt _confirmationPrompt;

		private int _activePanel = 0;
		private string _inputType = string.Empty;
		private string _inputActionToChange = string.Empty;
		private string _keybindToChange = string.Empty;

		public override void _Ready()
		{
			SetupBaseReferences();
			SetupReferences();
			SetupControlCategoryButtons();
			PopulateBreakableVariants();
			SetupActiveJoypadOptions();
			PopulateKeybindControls();
			DisplayActivePanel();
			PopulateLanguageList();
		}

		private void SetupReferences()
		{
			Input.JoyConnectionChanged += SetupActiveJoypadOptions;
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

			if (@event.IsActionPressed("ui_category_prev"))
			{
				SwitchActivePanel(false);
			}

			if (@event.IsActionPressed("ui_category_next"))
			{
				SwitchActivePanel(true);
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

		protected override void Focus()
		{
			if (Visible)
			{
				_focusTarget[0].GrabFocus();
				ChangeActivePanel(0);
			}
		}

		protected override void ToggleFocus()
		{
			_focusIndex = (_focusIndex < _focusTarget.Length - 1) ? _focusIndex + 1 : 0;

			if (_focusIndex == 0)
			{
				CategoryFocus();
			}
			else
			{
				_focusTarget[_focusIndex].GrabFocus();
			}
		}

		private void CategoryFocus()
		{
			_categoryFocusTargets[_activePanel].GrabFocus();
		}

		private void UpdateSettings()
		{
			_language.Selected = FindOptionIndex(_language, TranslationServer.GetLanguageName(refs.settings.Language));
			_font.Selected = refs.settings.Font;
			_controllerType.Selected = FindOptionIndex(_controllerType, refs.settings.ControlerPrompts);
			_livesDisplay.ButtonPressed = refs.settings.LivesAsText;
			_stageClearDisplay.ButtonPressed = refs.settings.StageClearScreen;
			_fullscreen.ButtonPressed = (refs.settings.ScreenMode > 0);
			_screenShake.ButtonPressed = refs.settings.ScreenShake;
			_pickupOrder.Selected = refs.settings.PickupOrder;
			_backgroundBrightness.Value = refs.settings.BackgroundBrightness;
			_helperTransparency.Value = refs.settings.HelperTransparency;
			_masterVolume.Value = refs.settings.MasterVolume;
			_musicVolume.Value = refs.settings.MusicVolume;
			_effectsVolume.Value = refs.settings.EffectsVolume;
			_mouseSpeed.Value = refs.settings.SpeedMouse;
			_keyboardSpeed.Value = refs.settings.SpeedKeyboard;
			_joypadSpeed.Value = refs.settings.SpeedController;
			int activeJoypadIndexHelper = FindOptionIndex(_activeJoypad, refs.settings.ActiveController);
			_activeJoypad.Selected = (activeJoypadIndexHelper < 0) ? 0 : activeJoypadIndexHelper;
			_vibrations.ButtonPressed = refs.settings.ControllerVibrations;

			UpdateVariants();
			UpdateKeybindings();
		}

		private void SaveSettings() // TODO: break into per-category methods
		{
			refs.settings.Language = TranslationServer.GetLoadedLocales()[_language.Selected];
			refs.settings.Font = _font.Selected;
			refs.settings.ControlerPrompts = _controllerType.GetItemText(_controllerType.Selected).ToLower();
			refs.settings.LivesAsText = _livesDisplay.ButtonPressed;
			refs.settings.StageClearScreen = _stageClearDisplay.ButtonPressed;
			refs.settings.ScreenMode = _fullscreen.ButtonPressed ? 3 : 0;
			refs.settings.ScreenShake = _screenShake.ButtonPressed;
			refs.settings.BackgroundBrightness = (float)_backgroundBrightness.Value;
			refs.settings.PickupOrder = _pickupOrder.Selected;
			refs.settings.HelperTransparency = (float)_helperTransparency.Value;
			refs.settings.MasterVolume = (float)_masterVolume.Value;
			refs.settings.MasterVolume = (float)_masterVolume.Value;
			refs.settings.MusicVolume = (float)_musicVolume.Value;
			refs.settings.EffectsVolume = (float)_effectsVolume.Value;
			refs.settings.SpeedMouse = (float)_mouseSpeed.Value;
			refs.settings.SpeedKeyboard = (float)_keyboardSpeed.Value;
			refs.settings.SpeedController = (float)_joypadSpeed.Value;
			refs.settings.ActiveController = (_activeJoypad.Selected >= 0) ? _activeJoypad.GetItemText(_activeJoypad.Selected) : string.Empty;
			refs.settings.ActiveControllerID = _activeJoypad.Selected - 1;
			refs.settings.ControllerVibrations = _vibrations.ButtonPressed;

			SaveBreakableVariants();
			refs.settings.SaveSettings();
			UpdateHeaderText();
			_confirmationPrompt.Activate();
		}

		private void CancelSettings()
		{
			refs.settings.LoadSettings();
			UpdateSettings();
		}

		private void RestoreDefaultSettings()
		{
			switch (_activePanel)
			{
				case 0:
					refs.settings.SetDefaultGeneralValues();
					break;

				case 1:
					refs.settings.SetDefaultVideoValues();
					break;

				case 2:
					refs.settings.SetDefaultAudioValues();
					break;

				case 3:
					refs.settings.SetDefaultControlValues();
					break;

				default:
					refs.settings.SetDefaultValues();
					break;
			}

			UpdateSettings();
		}

		private void ChangeActivePanel(int targetPanelIndex)
		{
			_activePanel = targetPanelIndex;
			DisplayActivePanel();
		}

		private void DisplayActivePanel()
		{
			for (int index = 1; index < GetChildCount() - StaticItems; index++)
			{
				((Control)GetChild(index)).Visible = ((index - 1) == _activePanel);
			}

			UpdateSettings();
			UpdateHeaderText();
			UpdateShortcutPrompts();
			CategoryFocus();
		}

		private void PopulateLanguageList()
		{
			List<string> languageNames = new List<string>();

			foreach (string languageCode in TranslationServer.GetLoadedLocales())
			{
				if (!languageNames.Contains(languageCode))
				{
					languageNames.Add(TranslationServer.GetLanguageName(languageCode));
				}
			}

			foreach (string languageName in languageNames)
			{
				_language.AddItem(languageName);
			}
		}

		private int FindOptionIndex(OptionButton targetList, string optionToFind)
		{
			for (int ind = 0; ind < targetList.ItemCount; ind++)
			{
				if (targetList.GetItemText(ind).ToLower() == optionToFind.ToLower())
				{
					return ind;
				}
			}

			return -1;
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

		private void UpdateVideoTextValues()
		{
			_backgroundBrightnessText.Text = $"{System.MathF.Round((float)_backgroundBrightness.Value * 100, 0)}%";
			_helperTransparencyText.Text = $"{System.MathF.Round((float)_helperTransparency.Value * 100, 0)}%";
		}

		private void UpdateAudioTextValues()
		{
			_masterVolumeText.Text = $"{System.MathF.Round((float)_masterVolume.Ratio * 100, 0)}%";
			_musicVolumeText.Text = $"{System.MathF.Round((float)_musicVolume.Ratio * 100, 0)}%";
			_effectsVolumeText.Text = $"{System.MathF.Round((float)_effectsVolume.Ratio * 100, 0)}%";
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

		private void ToggleControlCategory(Control target)
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

					string[] inputEvents = refs.localization.GetInputSymbol(inputAction.Name, inputAction.GetParent().Name);
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

			refs.settings.ChangeKeybinding(_inputActionToChange, _keybindToChange, @event);
			UpdateKeybindings();
		}

		private void ToggleBreakableVariantsDisplay()
		{
			_brekableVariantContainer.Visible = !_brekableVariantContainer.Visible;

			if (_brekableVariantContainer.Visible)
			{
				_brekableVariantContainer.GetChild(0).GetChild<Control>(2).GrabFocus();
			}
		}

		private void PopulateBreakableVariants()
		{
			foreach (BreakableVariant variant in refs.settings.BreakableVariants.Values)
			{
				Label breakableName = new Label
				{
					Text = $"BREAKABLE_{variant.TypeName.ToUpper()}",
					AutoTranslate = true,
					AnchorBottom = 1,
					AnchorRight = 0.3f,
					OffsetLeft = 8
				};

				Button pickerButton = new Button
				{
					AnchorBottom = 1,
					AnchorLeft = 0.3f,
					AnchorRight = 0.3f,
					IconAlignment = HorizontalAlignment.Center,
					Flat = true
				};

				pickerButton.Pressed += () => _colorPickerPanel.Activate(pickerButton);

				OptionButton variantSelector = new OptionButton
				{
					AnchorBottom = 1,
					AnchorLeft = 0.3f,
					AnchorRight = 1,
					OffsetLeft = 32,
					OffsetRight = -4
				};

				variantSelector.AddItem("BUTTON_DEFAULT");
				variantSelector.AddItem("BUTTON_DEFAULT_RIM");
				variantSelector.AddItem("BUTTON_CUSTOM");
				variantSelector.Select(variant.SpriteVariant);

				Control container = new Control
				{
					Name = variant.TypeName,
					CustomMinimumSize = new Vector2(0, 19)
				};

				variantSelector.ItemSelected += (index) => UpdateVariantControls(container);

				container.AddChild(breakableName);
				container.AddChild(pickerButton);
				container.AddChild(variantSelector);

				_brekableVariantContainer.AddChild(container);
			}

			Control spacer = new Control
			{
				Name = "spacer",
				CustomMinimumSize = new Vector2(0, 50)
			};

			_brekableVariantContainer.AddChild(spacer);

			_brekableVariantContainer.Visible = false;
		}

		private void SaveBreakableVariants()
		{
			foreach (Control variantContainer in _brekableVariantContainer.GetChildren())
			{
				if (variantContainer.Name != "spacer")
				{
					BreakableVariant targetVariant = refs.settings.BreakableVariants[variantContainer.Name];
					targetVariant.SpriteVariant = variantContainer.GetChild<OptionButton>(2).Selected;
					targetVariant.CustomColor = variantContainer.GetChild<Control>(1).Modulate;
					refs.settings.BreakableVariants[variantContainer.Name] = targetVariant;
				}
			}
		}

		private void UpdateVariants()
		{
			foreach (Control variantContainer in _brekableVariantContainer.GetChildren().Cast<Control>())
			{
				if (variantContainer.Name != "spacer")
				{
					variantContainer.GetChild<OptionButton>(2).Selected = refs.settings.BreakableVariants[variantContainer.Name].SpriteVariant;
					UpdateVariantControls(variantContainer);
				}
			}
		}

		private void UpdateVariantControls(Control variantContainer)
		{
			BreakableVariant variant = refs.settings.BreakableVariants[variantContainer.Name];
			string breakableIcon = $"res://assets/sprites/ui_elements/object_icons/{variant.TypeName}";
			int variantIndex = variantContainer.GetChild<OptionButton>(2).Selected;

			switch (variantIndex)
			{
				case 1:
					breakableIcon += "_rim.png";
					break;

				case 2:
					breakableIcon += "_alt.png";
					break;

				default:
					breakableIcon += ".png";
					break;
			}

			variantContainer.GetChild<Button>(1).Icon = ResourceLoader.Load<Texture2D>(breakableIcon);
			variantContainer.GetChild<Button>(1).Modulate = (variantIndex > 1) ? variant.CustomColor : new Color(1, 1, 1, 1);
			variantContainer.GetChild<Button>(1).Disabled = (variantIndex < 2);
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
				newPanelIndex = GetChildCount() - StaticItems - 2;
			}

			if (newPanelIndex >= GetChildCount() - StaticItems - 1)
			{
				newPanelIndex = 0;
			}

			ChangeActivePanel(newPanelIndex);
		}
	}
}