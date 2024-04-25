using Godot;

namespace BoGK.UI
{
	public partial class UICustomSessionPanel : UIPanel
	{
		[Export] private CheckButton _customSessionLengthButton;
		[Export] private Label _sessionLengthLabel;
		[Export] private HSlider _sessionLengthSlider;
		[Export] private CheckButton _shuffleStagesButton;

		public override void _Ready()
		{
			SetupBaseReferences();
			UpdateDisplayedValues();
		}

		protected override void Focus()
		{
			if (Visible)
			{
				UpdateDisplayedValues();

				if (_focusTarget.Length > 0)
				{
					_focusTarget[0].GrabFocus();
				}
			}
		}

		protected override void UpdateDisplayedValues()
		{
			_customSessionLengthButton.ButtonPressed = (refs.SessionLength > 0);
			ToggleCustomSessionLength(_customSessionLengthButton.ButtonPressed);
			_sessionLengthSlider.MaxValue = refs.gameData.Levels.Count;
			_sessionLengthSlider.Value = refs.gameData.Levels.Count;
			_shuffleStagesButton.ButtonPressed = refs.ShuffleStages;
		}

		private void ToggleCustomSessionLength(bool setActive)
		{
			_sessionLengthSlider.GetParent<Control>().Visible = setActive;
			_sessionLengthSlider.Editable = setActive;
		}

		private void SetSessionSettings()
		{
			refs.SessionLength = _customSessionLengthButton.ButtonPressed ? (int)_sessionLengthSlider.Value : -1;
			refs.ShuffleStages = _shuffleStagesButton.ButtonPressed;
			uiController.TogglePanel("GameSetupPanel");
		}

		private void UpdateSessionLengthText(float value)
		{
			_sessionLengthLabel.Text = $"{Tr("LABEL_SESSION_LENGTH")}: {(int)value}";
		}
	}
}
