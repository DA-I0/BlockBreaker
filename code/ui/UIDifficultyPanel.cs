using System;
using Godot;

namespace BoGK.UI
{
	public partial class UIDifficultyPanel : UIPaginatorPanel
	{
		[Export] private Label _name;
		[Export] private LineEdit _newName;
		[Export] private Label _livesMax;
		[Export] private HSlider _livesMaxSlider;
		[Export] private Label _livesStart;
		[Export] private HSlider _livesStartSlider;
		[Export] private Label _ballSpeed;
		[Export] private HSlider _ballSpeedSlider;
		[Export] private Label _ballAngleSpeed;
		[Export] private HSlider _ballAngleSlider;
		[Export] private Label _paddleSizeMax;
		[Export] private HSlider _paddleSizeMaxSlider;
		[Export] private Label _paddleSizeStart;
		[Export] private HSlider _paddleSizeStartSlider;
		[Export] private Label _paddleSizeMin;
		[Export] private HSlider _paddleSizeMinSlider;
		[Export] private Label _advancingSpeed;
		[Export] private CheckButton _advancingSpeedCheckButton;
		[Export] private Label _pickupSpeed;
		[Export] private HSlider _pickupSpeedSlider;

		[Export] private TextureButton _previousDifficulty;
		[Export] private TextureButton _nextDifficulty;

		[Export] private Control _nameEditControls;
		[Export] private TextureButton _deleteDifficulty;
		[Export] private TextureButton _editDifficulty;
		[Export] private TextureButton _addDifficulty;
		[Export] private Button _cancelEdit;

		[Export] private Control _focusHint;

		private int _currentDifficulty = 1;
		private bool _inEditMode = false;

		public override void _Ready()
		{
			SetupBaseReferences();
			CreateItemIndicators(refs.gameData.Difficulties.Count);
			ToggleEditorControls(false);
			UpdateEditorButtons();
		}

		protected override void Focus()
		{
			if (Visible)
			{
				_currentDifficulty = refs.SelectedDifficultyIndex;
				UpdateDisplayedValues();

				if (_inEditMode)
				{
					_newName.GrabFocus();
				}
				else
				{
					// _focusTarget[0].GrabFocus();
				}
			}
		}

		private void UpdateDisplayedValues()
		{
			if (_inEditMode)
			{
				ApplyEditorValues();
			}
			else
			{
				_currentDifficulty = (_currentDifficulty < 0) ? 1 : _currentDifficulty;
				ApplyStaticValues();
			}

			UpdatePaginatorStatus(_currentDifficulty);
			TogglePaginationButtons();
		}

		private void EnableEditor(bool newDifficulty)
		{
			ApplyDifficultyToEditor(newDifficulty);
			ToggleEditorControls(true);
			Focus();
		}

		private void ToggleEditorControls(bool enable)
		{
			_inEditMode = enable;

			_name.Visible = !_inEditMode;
			_nameEditControls.Visible = _inEditMode;
			_livesMaxSlider.Visible = _inEditMode;
			_livesStartSlider.Visible = _inEditMode;
			_ballSpeedSlider.Visible = _inEditMode;
			_ballAngleSlider.Visible = _inEditMode;
			_paddleSizeMaxSlider.Visible = _inEditMode;
			_paddleSizeStartSlider.Visible = _inEditMode;
			_paddleSizeMinSlider.Visible = _inEditMode;
			_advancingSpeedCheckButton.Visible = _inEditMode;
			_pickupSpeedSlider.Visible = _inEditMode;
			_paginatorParent.Visible = !_inEditMode;
			_focusHint.Visible = _inEditMode;

			UpdateEditorButtons();
			UpdateDisplayedValues();
		}

		private void ApplyEditorValues() // might have to do manual rounding for ball and angle speed, sometimes returns a lot of decimals
		{
			_livesMax.Text = $"{Tr("DIFF_LIVES_MAX")}: {_livesMaxSlider.Value}";
			_livesStart.Text = $"{Tr("DIFF_LIVES_START")}: {_livesStartSlider.Value}";
			_ballSpeed.Text = $"{Tr("DIFF_BALL_SPEED")}: {MathF.Round((float)_ballSpeedSlider.Value, 2)}x";
			_ballAngleSpeed.Text = $"{Tr("DIFF_ANGLE_SPEED")}: {MathF.Round((float)_ballAngleSlider.Value, 2)}x";
			_paddleSizeMax.Text = $"{Tr("DIFF_PADDLE_SIZE_MAX")}: {_paddleSizeMaxSlider.Value}";
			_paddleSizeStart.Text = $"{Tr("DIFF_PADDLE_SIZE_START")}: {_paddleSizeStartSlider.Value}";
			_paddleSizeMin.Text = $"{Tr("DIFF_PADDLE_SIZE_MIN")}: {_paddleSizeMinSlider.Value}";
			_advancingSpeed.Text = $"{Tr("DIFF_ADVANCING_SPEED")}:";
			_pickupSpeed.Text = $"{Tr("DIFF_PICKUP_SPEED")}: {MathF.Round((float)_pickupSpeedSlider.Value, 2)}x";
		}

		private void ApplyStaticValues()
		{
			if (refs.gameData.Difficulties[_currentDifficulty].DifficultyName.Contains("DIFF_DEFAULT_"))
			{
				_name.Text = Tr(refs.gameData.Difficulties[_currentDifficulty].DifficultyName);
			}
			else
			{
				_name.Text = refs.gameData.Difficulties[_currentDifficulty].DifficultyName;
			}

			_livesMax.Text = $"{Tr("DIFF_LIVES_MAX")}: {refs.gameData.Difficulties[_currentDifficulty].MaxLives}";
			_livesStart.Text = $"{Tr("DIFF_LIVES_START")}: {refs.gameData.Difficulties[_currentDifficulty].StartingLives}";
			_ballSpeed.Text = $"{Tr("DIFF_BALL_SPEED")}: {refs.gameData.Difficulties[_currentDifficulty].BallSpeedMultiplier}x";
			_ballAngleSpeed.Text = $"{Tr("DIFF_ANGLE_SPEED")}: {refs.gameData.Difficulties[_currentDifficulty].AngleSelectSpeed}x";
			_paddleSizeMax.Text = $"{Tr("DIFF_PADDLE_SIZE_MAX")}: {refs.gameData.Difficulties[_currentDifficulty].MaxPaddleSize}";
			_paddleSizeStart.Text = $"{Tr("DIFF_PADDLE_SIZE_START")}: {refs.gameData.Difficulties[_currentDifficulty].StartPaddleSize}";
			_paddleSizeMin.Text = $"{Tr("DIFF_PADDLE_SIZE_MIN")}: {refs.gameData.Difficulties[_currentDifficulty].MinPaddleSize}";
			_advancingSpeed.Text = $"{Tr("DIFF_ADVANCING_SPEED")}: {refs.gameData.Difficulties[_currentDifficulty].AdvancingSpeed}";
			_pickupSpeed.Text = $"{Tr("DIFF_PICKUP_SPEED")}: {refs.gameData.Difficulties[_currentDifficulty].PickupSpeedMultiplier}x";
		}

		private void ApplyDifficultyToEditor(bool newDifficulty)
		{
			int helperIndex = _currentDifficulty;
			_inEditMode = true;

			if (newDifficulty)
			{
				_currentDifficulty = -1;
				helperIndex = 1;
			}

			_newName.Text = newDifficulty ? Tr("PLACEHOLDER_DIFF_NAME") : refs.gameData.Difficulties[_currentDifficulty].DifficultyName;
			_livesMaxSlider.Value = refs.gameData.Difficulties[helperIndex].MaxLives;
			_livesStartSlider.Value = refs.gameData.Difficulties[helperIndex].StartingLives;
			_ballSpeedSlider.Value = refs.gameData.Difficulties[helperIndex].BallSpeedMultiplier;
			_ballAngleSlider.Value = refs.gameData.Difficulties[helperIndex].AngleSelectSpeed;
			_paddleSizeMaxSlider.Value = refs.gameData.Difficulties[helperIndex].MaxPaddleSize;
			_paddleSizeStartSlider.Value = refs.gameData.Difficulties[helperIndex].StartPaddleSize;
			_paddleSizeMinSlider.Value = refs.gameData.Difficulties[helperIndex].MinPaddleSize;
			_advancingSpeedCheckButton.ButtonPressed = refs.gameData.Difficulties[helperIndex].AdvancingSpeed;
			_pickupSpeedSlider.Value = refs.gameData.Difficulties[helperIndex].PickupSpeedMultiplier;
		}

		private void UpdateEditorValues(float value)
		{
			_livesStartSlider.MaxValue = _livesMaxSlider.Value;
			_paddleSizeMaxSlider.MinValue = _paddleSizeMinSlider.Value;
			_paddleSizeMinSlider.MaxValue = _paddleSizeMaxSlider.Value;
			_paddleSizeStartSlider.MaxValue = _paddleSizeMaxSlider.Value;
			_paddleSizeStartSlider.MinValue = _paddleSizeMinSlider.Value;

			UpdateDisplayedValues();
		}

		private void ChangeDifficulty(bool next)
		{
			_currentDifficulty += next ? 1 : -1;
			CheckDifficultyRange();
			UpdateDisplayedValues();
			UpdateEditorButtons();
		}

		private void CheckDifficultyRange()
		{
			if (_currentDifficulty > refs.gameData.Difficulties.Count - 1)
			{
				_currentDifficulty = 0;
			}

			if (_currentDifficulty < 0)
			{
				_currentDifficulty = refs.gameData.Difficulties.Count - 1;
			}
		}

		private void UpdateEditorButtons()
		{
			bool showEditButtons = (_currentDifficulty >= refs.gameData.DefaultDifficultyCount) && !_inEditMode;

			_deleteDifficulty.Visible = showEditButtons;
			_editDifficulty.Visible = showEditButtons;
			_addDifficulty.Visible = !_inEditMode;

			_cancelEdit.Visible = _inEditMode;
		}

		private void Confirm()
		{
			if (_inEditMode)
			{
				SaveDifficulty();
			}
			else
			{
				if (_addDifficulty.HasFocus() || _editDifficulty.HasFocus() || _deleteDifficulty.HasFocus())
				{
					return;
				}

				SelectDifficulty();
			}
		}

		private void SelectDifficulty()
		{
			refs.SetDifficulty(_currentDifficulty);
			uiController.TogglePanel("GameSetupPanel");
		}

		private void SaveDifficulty()
		{
			if (_newName.Text.Trim() == string.Empty)
			{
				return;
			}

			Models.Difficulty newDifficulty = new Models.Difficulty(
				_newName.Text.Trim(),
				(int)_livesMaxSlider.Value,
				(int)_livesStartSlider.Value,
				MathF.Round((float)_ballSpeedSlider.Value, 2),
				MathF.Round((float)_ballAngleSlider.Value, 2),
				(int)_paddleSizeMaxSlider.Value,
				(int)_paddleSizeStartSlider.Value,
				(int)_paddleSizeMinSlider.Value,
				(bool)_advancingSpeedCheckButton.ButtonPressed,
				(float)_pickupSpeedSlider.Value
			);

			if (_currentDifficulty < 0)
			{
				FileOperations.SaveDifficulty(string.Empty, newDifficulty);
				refs.gameData.AddDifficulty(newDifficulty);
				_currentDifficulty = refs.gameData.Difficulties.Count - 1;
			}
			else
			{
				FileOperations.SaveDifficulty(refs.gameData.Difficulties[_currentDifficulty].DifficultyName, newDifficulty);
				refs.gameData.UpdateDifficulty(_currentDifficulty, newDifficulty);
			}

			CreateItemIndicators(refs.gameData.Difficulties.Count);
			ToggleEditorControls(false);
		}

		private void DeleteDifficulty()
		{
			FileOperations.DeleteDifficulty(refs.gameData.Difficulties[_currentDifficulty].DifficultyName);
			refs.gameData.RemoveDifficulty(_currentDifficulty);
			CreateItemIndicators(refs.gameData.Difficulties.Count);
			ChangeDifficulty(false);
		}

		private void TogglePaginationButtons()
		{
			_previousDifficulty.Disabled = _inEditMode;
			_nextDifficulty.Disabled = _inEditMode;
		}
	}
}