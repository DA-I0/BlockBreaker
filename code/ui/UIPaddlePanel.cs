using Godot;

namespace BoGK.UI
{
	public partial class UIPaddlePanel : UIPaginatorPanel
	{
		[Export] private TextureRect _paddleSprite;
		[Export] private Label _paddleName;
		[Export] private Label _paddleDescription;

		private int _currentPaddle = 1;
		private int _paddleCount = 0;

		public override void _Ready()
		{
			SetupBaseReferences();
			_paddleCount = DirAccess.GetFilesAt("res://prefabs/paddles/").Length;
			CreateItemIndicators(_paddleCount);
			UpdateDisplayedValues();
		}

		protected override void Focus()
		{
			if (_focusTarget.Length > 0)
			{
				_currentPaddle = refs.SelectedPaddleIndex;
				_focusTarget[0].GrabFocus();
			}
		}

		protected override void UpdateDisplayedValues()
		{
			if (ResourceLoader.Exists($"res://assets/sprites/paddles/paddle_{_currentPaddle}_icon.png"))
			{
				_paddleSprite.Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/paddles/paddle_{_currentPaddle}_icon.png");
			}
			else
			{
				_paddleSprite.Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/paddles/paddle_{_currentPaddle}.png");
			}
			_paddleName.Text = $"PADDLE_{_currentPaddle}_NAME";
			_paddleDescription.Text = $"PADDLE_{_currentPaddle}_DESC";
			UpdatePaginatorStatus(_currentPaddle - 1);
		}

		private void ChangePaddle(bool next)
		{
			_currentPaddle += next ? 1 : -1;
			CheckPaddleRange();
			UpdateDisplayedValues();
		}

		private void CheckPaddleRange()
		{
			if (_currentPaddle > _paddleCount)
			{
				_currentPaddle = 1;
			}

			if (_currentPaddle < 1)
			{
				_currentPaddle = _paddleCount;
			}
		}

		private void SelectPaddle()
		{
			refs.SetPaddle(_currentPaddle);
			uiController.TogglePanel("GameSetupPanel");
		}
	}
}