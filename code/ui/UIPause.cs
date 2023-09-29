using Godot;

public partial class UIPause : Control
{
	SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		refs.GameStateChanged += TogglePausePanel;
		TogglePausePanel();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("game_pause") && refs.CurrentGameState != GameState.menu)
		{
			if (refs.CurrentGameState == GameState.gameplay)
			{
				refs.ChangeGameState(GameState.pause);
			}
			else
			{
				refs.ChangeGameState(GameState.gameplay);
			}
		}
	}

	private void TogglePausePanel()
	{
		Visible = (refs.CurrentGameState == GameState.pause);
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
