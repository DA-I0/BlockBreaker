using System.Linq;
using BoGK.Models;
using Godot;

namespace BoGK.UI
{
	public partial class UIOptionsVideoPanel : UIOptionsBaseCategory
	{
		[Export] private CheckButton _fullscreen;
		[Export] private CheckButton _screenShake;
		[Export] private OptionButton _pickupOrder;
		[Export] private Label _backgroundBrightnessText;
		[Export] private OptionButton _backgroundColorPalette;
		[Export] private HSlider _backgroundBrightness;
		[Export] private Label _effectTransparencyText;
		[Export] private HSlider _effectTransparency;
		[Export] private OptionButton _breakableColorPalette;
		[Export] private Control _breakableVariantContainer;
		[Export] private UIColorPicker _colorPickerPanel;

		public override void _Ready()
		{
			SetupBaseReferences();
			SetupReferences();
		}

		public override void Disable()
		{
			Visible = false;
			FoldExpandableControls();
		}

		public override void UpdateSettings()
		{
			_fullscreen.ButtonPressed = (Refs.settings.ScreenMode > 0);
			_screenShake.ButtonPressed = Refs.settings.ScreenShake;
			_pickupOrder.Selected = Refs.settings.PickupOrder;
			_backgroundColorPalette.Selected = GameSystem.HelperMethods.FindOptionIndex(_backgroundColorPalette, $"OPTION_VIDEO_PALETTE_{Refs.settings.BackgroundColorPalette}");
			_backgroundBrightness.Value = Refs.settings.BackgroundBrightness;
			_effectTransparency.Value = Refs.settings.EffectTransparency;
			_breakableColorPalette.Selected = GameSystem.HelperMethods.FindOptionIndex(_breakableColorPalette, $"OPTION_VIDEO_PALETTE_{Refs.settings.InteractableColorPalette}");

			UpdateVariants();
		}

		public override void ApplySettings()
		{
			Refs.settings.ScreenMode = _fullscreen.ButtonPressed ? 3 : 0;
			Refs.settings.ScreenShake = _screenShake.ButtonPressed;
			Refs.settings.BackgroundColorPalette = _backgroundColorPalette.GetItemText(_backgroundColorPalette.Selected).Split("_")[^1].ToLower();
			Refs.settings.BackgroundBrightness = (float)_backgroundBrightness.Value;
			Refs.settings.PickupOrder = _pickupOrder.Selected;
			Refs.settings.EffectTransparency = (float)_effectTransparency.Value;
			Refs.settings.InteractableColorPalette = _breakableColorPalette.GetItemText(_breakableColorPalette.Selected).Split("_")[^1].ToLower();

			SaveBreakableVariants();
		}

		private void CancelSettings()
		{
			Refs.settings.LoadSettings();
			UpdateSettings();
		}

		public override void RestoreDefaultSettings()
		{
			Refs.settings.SetDefaultVideoValues();
		}

		private void SetupReferences()
		{
			_mainPanel.FinalizeSetup += PopulateBreakableVariants;
			_mainPanel.FinalizeSetup += PopulateBackgroundColorPalettes;
			_backgroundBrightness.ValueChanged += UpdateTextValues;
			_effectTransparency.ValueChanged += UpdateTextValues;
		}

		private void UpdateTextValues(double value)
		{
			_backgroundBrightnessText.Text = $"{System.MathF.Round((float)_backgroundBrightness.Value * 100, 0)}%";
			_effectTransparencyText.Text = $"{System.MathF.Round((float)_effectTransparency.Value * 100, 0)}%";
		}

		private void PopulateBackgroundColorPalettes()
		{
			_backgroundColorPalette.Clear();
			_breakableColorPalette.Clear();

			foreach (string palette in GameSystem.FileOperations.GetFolderList(ProjectSettings.GetSetting("global/TilesetFolderPath").ToString()))
			{
				_backgroundColorPalette.AddItem($"OPTION_VIDEO_PALETTE_{palette.ToUpper()}");
				_breakableColorPalette.AddItem($"OPTION_VIDEO_PALETTE_{palette.ToUpper()}");
			}
		}

		private void ToggleBreakableVariantsDisplay()
		{
			_breakableVariantContainer.Visible = !_breakableVariantContainer.Visible;

			if (_breakableVariantContainer.Visible)
			{
				_breakableVariantContainer.GetChild(0).GetChild<Control>(-1).GrabFocus();
			}
		}

		private void PopulateBreakableVariants()
		{
			foreach (BreakableVariant variant in Refs.settings.BreakableVariants.Values)
			{
				Label breakableName = new Label
				{
					Text = $"BREAKABLE_{variant.TypeName.ToUpper()}",
					AutoTranslate = true,
					AnchorBottom = 1,
					AnchorRight = 0.6f,
					OffsetLeft = 8
				};

				Button pickerButton = new Button
				{
					AnchorBottom = 1,
					AnchorLeft = 0.5f,
					AnchorRight = 0.5f,
					OffsetLeft = -4,
					OffsetRight = -4,
					IconAlignment = HorizontalAlignment.Center,
					Flat = true
				};

				pickerButton.Pressed += () => _colorPickerPanel.Activate(pickerButton);

				OptionButton variantSelector = new OptionButton
				{
					AnchorBottom = 1,
					AnchorLeft = 0.6f,
					AnchorRight = 1,
					OffsetLeft = -3,
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

				_breakableVariantContainer.AddChild(container);
			}

			Control spacer = new Control
			{
				Name = "spacer",
				CustomMinimumSize = new Vector2(0, 50)
			};

			_breakableVariantContainer.AddChild(spacer);

			_breakableVariantContainer.Visible = false;
		}

		private void SaveBreakableVariants()
		{
			foreach (Control variantContainer in _breakableVariantContainer.GetChildren())
			{
				if (variantContainer.Name != "spacer" && !variantContainer.Name.ToString().Contains("Setting"))
				{
					BreakableVariant targetVariant = Refs.settings.BreakableVariants[variantContainer.Name];
					targetVariant.SpriteVariant = variantContainer.GetChild<OptionButton>(2).Selected;
					targetVariant.CustomColor = variantContainer.GetChild<Control>(1).Modulate;
					Refs.settings.BreakableVariants[variantContainer.Name] = targetVariant;
				}
			}
		}

		private void UpdateVariants()
		{
			foreach (Control variantContainer in _breakableVariantContainer.GetChildren().Cast<Control>())
			{
				if (variantContainer.Name != "spacer" && !variantContainer.Name.ToString().Contains("Setting"))
				{
					variantContainer.GetChild<OptionButton>(2).Selected = Refs.settings.BreakableVariants[variantContainer.Name].SpriteVariant;
					UpdateVariantControls(variantContainer);
				}
			}
		}

		private void UpdateVariantControls(Control variantContainer)
		{
			BreakableVariant variant = Refs.settings.BreakableVariants[variantContainer.Name];
			string iconPalette = _breakableColorPalette.GetItemText(_breakableColorPalette.Selected).Split("_")[^1].ToLower();
			string breakableIcon = $"{ProjectSettings.GetSetting("global/BreakableIconsFilePath")}/{iconPalette}/{variant.TypeName}";
			int variantIndex = variantContainer.GetChild<OptionButton>(2).Selected;

			switch (variantIndex)
			{
				case 1:
					breakableIcon += "_rim.png";
					break;

				case 2:
					breakableIcon = $"{breakableIcon.Replace(iconPalette, "custom")}.png";
					break;

				default:
					breakableIcon += ".png";
					break;
			}

			variantContainer.GetChild<Button>(1).Icon = ResourceLoader.Load<Texture2D>(breakableIcon);
			variantContainer.GetChild<Button>(1).Modulate = (variantIndex > 1) ? variant.CustomColor : new Color(1, 1, 1, 1);
			variantContainer.GetChild<Button>(1).Disabled = (variantIndex < 2);
		}

		private void FoldExpandableControls()
		{
			_breakableVariantContainer.Visible = false;
		}
	}
}
