using System;
using Godot;

public partial class UIDifficultyPanel : UIPanel
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

	[Export] private Control _nameEditControls;
	[Export] private TextureButton _deleteDifficulty;
	[Export] private TextureButton _editDifficulty;
	[Export] private TextureButton _addDifficulty;
	[Export] private Button _cancelEdit;

	private int _currentDifficulty = 1;
	private bool _inEditMode = false;


	public override void _Ready()
	{
		SetupReferences();
		ToggleEditorControls(false);
		UpdateDisplayedValues();
		UpdateEditorButtons();
	}

	protected override void Focus()
	{
		if (Visible)
		{
			if (_inEditMode)
			{
				_newName.GrabFocus();
			}
			else
			{
				_focusTarget.GrabFocus();
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
	}

	private void EnableEditor(bool newDifficulty)
	{
		ApplyDifficultyToEditor(newDifficulty);
		ToggleEditorControls(true);
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

		UpdateEditorButtons();
		UpdateDisplayedValues();
	}

	private void ApplyEditorValues() // might have to do manual rounding for ball and angle speed, sometimes returns a lot of decimals
	{
		_livesMax.Text = $"{Tr("diff_lives_max")}: {_livesMaxSlider.Value}";
		_livesStart.Text = $"{Tr("diff_lives_start")}: {_livesStartSlider.Value}";
		_ballSpeed.Text = $"{Tr("diff_ball_speed")}: {MathF.Round((float)_ballSpeedSlider.Value, 2)}";
		_ballAngleSpeed.Text = $"{Tr("diff_angle_speed")}: {MathF.Round((float)_ballAngleSlider.Value, 2)}";
		_paddleSizeMax.Text = $"{Tr("diff_paddle_size_max")}: {_paddleSizeMaxSlider.Value}";
		_paddleSizeStart.Text = $"{Tr("diff_paddle_size_start")}: {_paddleSizeStartSlider.Value}";
		_paddleSizeMin.Text = $"{Tr("diff_paddle_size_min")}: {_paddleSizeMinSlider.Value}";
	}

	private void ApplyStaticValues()
	{
		_name.Text = refs.gameData.Difficulties[_currentDifficulty].DifficultyName;
		_livesMax.Text = $"{Tr("diff_lives_max")}: {refs.gameData.Difficulties[_currentDifficulty].MaxLives}";
		_livesStart.Text = $"{Tr("diff_lives_start")}: {refs.gameData.Difficulties[_currentDifficulty].StartingLives}";
		_ballSpeed.Text = $"{Tr("diff_ball_speed")}: {refs.gameData.Difficulties[_currentDifficulty].BallSpeedMultiplier}";
		_ballAngleSpeed.Text = $"{Tr("diff_angle_speed")}: {refs.gameData.Difficulties[_currentDifficulty].AngleSelectSpeed}";
		_paddleSizeMax.Text = $"{Tr("diff_paddle_size_max")}: {refs.gameData.Difficulties[_currentDifficulty].MaxPaddleSize}";
		_paddleSizeStart.Text = $"{Tr("diff_paddle_size_start")}: {refs.gameData.Difficulties[_currentDifficulty].StartPaddleSize}";
		_paddleSizeMin.Text = $"{Tr("diff_paddle_size_min")}: {refs.gameData.Difficulties[_currentDifficulty].MinPaddleSize}";
	}

	private void ApplyDifficultyToEditor(bool newDifficulty)
	{
		int helperIndex = _currentDifficulty;

		if (newDifficulty)
		{
			_currentDifficulty = -1;
			helperIndex = 1;
		}

		_newName.Text = newDifficulty ? Tr("diff_placeholder_name") : refs.gameData.Difficulties[_currentDifficulty].DifficultyName;
		_livesMaxSlider.Value = refs.gameData.Difficulties[helperIndex].MaxLives;
		_livesStartSlider.Value = refs.gameData.Difficulties[helperIndex].StartingLives;
		_ballSpeedSlider.Value = refs.gameData.Difficulties[helperIndex].BallSpeedMultiplier;
		_ballAngleSlider.Value = refs.gameData.Difficulties[helperIndex].AngleSelectSpeed;
		_paddleSizeMaxSlider.Value = refs.gameData.Difficulties[helperIndex].MaxPaddleSize;
		_paddleSizeStartSlider.Value = refs.gameData.Difficulties[helperIndex].StartPaddleSize;
		_paddleSizeMinSlider.Value = refs.gameData.Difficulties[helperIndex].MinPaddleSize;
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
			SelectDifficulty();
		}
	}

	private void SelectDifficulty()
	{
		refs.SetDifficulty(_currentDifficulty);
		uiController.TogglePanel("LevelPanel");
	}

	public void SaveDifficulty()
	{
		if (_newName.Text.Trim() == string.Empty)
		{
			return;
		}

		Difficulty newDifficulty = new Difficulty(
			_newName.Text.Trim(),
			(int)_livesMaxSlider.Value,
			(int)_livesStartSlider.Value,
			MathF.Round((float)_ballSpeedSlider.Value, 2),
			MathF.Round((float)_ballAngleSlider.Value, 2),
			(int)_paddleSizeMaxSlider.Value,
			(int)_paddleSizeStartSlider.Value,
			(int)_paddleSizeMinSlider.Value
		);


		if (_currentDifficulty < 0)
		{
			refs.fileOperations.SaveDifficulty(string.Empty, newDifficulty);
			refs.gameData.AddDifficulty(newDifficulty);
			_currentDifficulty = refs.gameData.Difficulties.Count - 1;
		}
		else
		{
			refs.fileOperations.SaveDifficulty(refs.gameData.Difficulties[_currentDifficulty].DifficultyName, newDifficulty);
			refs.gameData.UpdateDifficulty(_currentDifficulty, newDifficulty);
		}

		ToggleEditorControls(false);
	}

	public void DeleteDifficulty()
	{
		refs.fileOperations.DeleteDifficulty(refs.gameData.Difficulties[_currentDifficulty].DifficultyName);
		refs.gameData.RemoveDifficulty(_currentDifficulty);
		ChangeDifficulty(false);
	}
}
