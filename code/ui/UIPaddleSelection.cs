using Godot;

public partial class UIPaddleSelection : Panel
{
	[Export] private TextureRect _paddleSprite;
	private int _currentPaddle = 1;

	private SessionController refs;
	private UIController uiController;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		uiController = (UIController)GetNode("../..");
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
