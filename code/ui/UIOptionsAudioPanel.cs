using Godot;

namespace BoGK.UI
{
	public partial class UIOptionsAudioPanel : UIOptionsBaseCategory
	{
		[Export] private Label _masterVolumeText;
		[Export] private HSlider _masterVolume;
		[Export] private Label _musicVolumeText;
		[Export] private HSlider _musicVolume;
		[Export] private Label _effectsVolumeText;
		[Export] private HSlider _effectsVolume;

		public override void _Ready()
		{
			SetupBaseReferences();
			SetupReferences();
		}

		public override void UpdateSettings()
		{
			_masterVolume.Value = Refs.settings.MasterVolume;
			_musicVolume.Value = Refs.settings.MusicVolume;
			_effectsVolume.Value = Refs.settings.EffectsVolume;
		}

		public override void ApplySettings()
		{
			Refs.settings.MasterVolume = (float)_masterVolume.Value;
			Refs.settings.MasterVolume = (float)_masterVolume.Value;
			Refs.settings.MusicVolume = (float)_musicVolume.Value;
			Refs.settings.EffectsVolume = (float)_effectsVolume.Value;
		}

		public override void RestoreDefaultSettings()
		{
			Refs.settings.SetDefaultAudioValues();
		}

		protected override void ToggleFocus()
		{
			_focusIndex = (_focusIndex < _focusTarget.Length - 1) ? _focusIndex + 1 : 0;
			_focusTarget[_focusIndex].GrabFocus();
		}

		private void SetupReferences()
		{
			_masterVolume.ValueChanged += UpdateTextValues;
			_musicVolume.ValueChanged += UpdateTextValues;
			_effectsVolume.ValueChanged += UpdateTextValues;
		}

		private void UpdateTextValues(double value)
		{
			_masterVolumeText.Text = $"{System.MathF.Round((float)_masterVolume.Ratio * 100, 0)}%";
			_musicVolumeText.Text = $"{System.MathF.Round((float)_musicVolume.Ratio * 100, 0)}%";
			_effectsVolumeText.Text = $"{System.MathF.Round((float)_effectsVolume.Ratio * 100, 0)}%";
		}
	}
}
