public enum GameState { game, menu, pause };

public enum PickupType { ball_fast, ball_multi, ball_small, ghost, life, paddle_thin, paddle_wide, safetyNet };

[System.Serializable]
public struct SettingsFile
{
	public bool fullScreen;
	public int screenWidth;
	public int screenHeight;
	public float volume;
	public int speedMouse;
	public int speedKeyboard;
}