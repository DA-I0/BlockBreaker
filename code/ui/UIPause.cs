using Godot;

namespace BoGK.UI
{
	public partial class UIPause : Control
	{
		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.GameStateChanged += TogglePausePanel;
			TogglePausePanel(GameState.gameplay);
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionReleased("game_pause"))
			{
				switch (refs.CurrentGameState)
				{
					case GameState.gameplay:
						refs.ChangeGameState(GameState.pause);
						break;

					case GameState.pause:
						refs.ChangeGameState(GameState.gameplay);
						break;

					default:
						break;
				}
			}
		}

		private void TogglePausePanel(GameState newState)
		{
			Visible = (newState == GameState.pause);
			Focus();
		}

		private void ReturnToMenu()
		{
			refs.levelManager.LoadMenuScene();
		}

		private void ReturnToGame()
		{
			refs.ChangeGameState(GameState.gameplay);
		}

		private void Focus()
		{
			if (Visible)
			{
				((Button)FindChild("ButtonConfirm")).GrabFocus();
			}
		}
	}
}