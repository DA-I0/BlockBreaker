using Godot;

namespace BoGK.UI
{
	public partial class UICustomSessionPanel : UIPanel
	{
		[Export] private CheckButton _customSessionLengthButton;
		[Export] private Label _sessionLengthLabel;
		[Export] private HSlider _sessionLengthSlider;
		[Export] private CheckButton _shuffleStagesButton;
		[Export] private CheckButton _disablePickupsButton;
		[Export] private CheckButton _disappearingBallButton;

		public override void _Ready()
		{
			SetupBaseReferences();
			SetupNavigation();
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
			_sessionLengthSlider.Value = (refs.SessionLength > 0) ? refs.SessionLength : refs.gameData.Levels.Count;
			_shuffleStagesButton.ButtonPressed = refs.ShuffleStages;
			_disablePickupsButton.ButtonPressed = refs.DisablePickups;
			_disappearingBallButton.ButtonPressed = refs.DisappearingBall;
		}

		private void SetupNavigation()
		{
			Control parent = GetNode<Control>("Options");

			for (int index = 0; index < parent.GetChildCount(); index++)
			{
				parent.GetChild(index).GetChild<Control>(1).FocusNeighborLeft = parent.GetChild(index).GetChild(1).GetPath();
				parent.GetChild(index).GetChild<Control>(1).FocusNeighborRight = parent.GetChild(index).GetChild(1).GetPath();
			}

			parent.GetChild(0).GetChild<Control>(1).FocusNeighborTop = GetNode("ConfirmSettings").GetPath();
			parent.GetChild(parent.GetChildCount() - 1).GetChild<Control>(1).FocusNeighborBottom = GetNode("ConfirmSettings").GetPath();

			GetNode<Control>("ConfirmSettings").FocusNeighborTop = parent.GetChild(parent.GetChildCount() - 1).GetChild(1).GetPath();
			GetNode<Control>("ConfirmSettings").FocusNeighborBottom = parent.GetChild(0).GetChild(1).GetPath();
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
			refs.DisablePickups = _disablePickupsButton.ButtonPressed;
			refs.DisappearingBall = _disappearingBallButton.ButtonPressed;
			uiController.TogglePanel("GameSetupPanel");
		}

		private void UpdateSessionLengthText(float value)
		{
			_sessionLengthLabel.Text = $"{Tr("LABEL_SESSION_LENGTH")}: {(int)value}";
		}
	}
}
