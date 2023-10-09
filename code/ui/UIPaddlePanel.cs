using Godot;

public partial class UIPaddlePanel : UIPanel
{
	[Export] private TextureRect _paddleSprite;
	private int _currentPaddle = 1;

	public override void _Ready()
	{
		SetupReferences();
		UpdateDisplayedValues();
	}

	private void UpdateDisplayedValues()
	{
		_paddleSprite.Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/paddles/paddle_{_currentPaddle}.png");
	}

	private void ChangePaddle(bool next)
	{
		_currentPaddle += next ? 1 : -1;
		CheckPaddleRange();
		UpdateDisplayedValues();
	}

	private void CheckPaddleRange()
	{
		if (_currentPaddle > 3)
		{
			_currentPaddle = 1;
		}

		if (_currentPaddle < 1)
		{
			_currentPaddle = 3;
		}
	}

	private void SelectPaddle()
	{
		refs.SetPaddle(_currentPaddle);
		uiController.TogglePanel("DifficultyPanel");
	}
}
