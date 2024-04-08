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
		[Export] private CheckButton _colorPalette;
		[Export] private HSlider _backgroundBrightness;
		[Export] private Label _effectTransparencyText;
		[Export] private HSlider _effectTransparency;
		[Export] private Control _brekableVariantContainer;
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
			_colorPalette.ButtonPressed = Refs.settings.UseAlternativeColorPalette;
			_backgroundBrightness.Value = Refs.settings.BackgroundBrightness;
			_effectTransparency.Value = Refs.settings.EffectTransparency;

			UpdateVariants();
		}

		public override void ApplySettings()
		{
			Refs.settings.ScreenMode = _fullscreen.ButtonPressed ? 3 : 0;
			Refs.settings.ScreenShake = _screenShake.ButtonPressed;
			Refs.settings.UseAlternativeColorPalette = _colorPalette.ButtonPressed;
			Refs.settings.BackgroundBrightness = (float)_backgroundBrightness.Value;
			Refs.settings.PickupOrder = _pickupOrder.Selected;
			Refs.settings.EffectTransparency = (float)_effectTransparency.Value;

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
			_backgroundBrightness.ValueChanged += UpdateTextValues;
			_effectTransparency.ValueChanged += UpdateTextValues;
		}

		private void UpdateTextValues(double value)
		{
			_backgroundBrightnessText.Text = $"{System.MathF.Round((float)_backgroundBrightness.Value * 100, 0)}%";
			_effectTransparencyText.Text = $"{System.MathF.Round((float)_effectTransparency.Value * 100, 0)}%";
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
			foreach (BreakableVariant variant in Refs.settings.BreakableVariants.Values)
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
					BreakableVariant targetVariant = Refs.settings.BreakableVariants[variantContainer.Name];
					targetVariant.SpriteVariant = variantContainer.GetChild<OptionButton>(2).Selected;
					targetVariant.CustomColor = variantContainer.GetChild<Control>(1).Modulate;
					Refs.settings.BreakableVariants[variantContainer.Name] = targetVariant;
				}
			}
		}

		private void UpdateVariants()
		{
			foreach (Control variantContainer in _brekableVariantContainer.GetChildren().Cast<Control>())
			{
				if (variantContainer.Name != "spacer")
				{
					variantContainer.GetChild<OptionButton>(2).Selected = Refs.settings.BreakableVariants[variantContainer.Name].SpriteVariant;
					UpdateVariantControls(variantContainer);
				}
			}
		}

		private void UpdateVariantControls(Control variantContainer)
		{
			BreakableVariant variant = Refs.settings.BreakableVariants[variantContainer.Name];
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

		private void FoldExpandableControls()
		{
			_brekableVariantContainer.Visible = false;
		}
	}
}
