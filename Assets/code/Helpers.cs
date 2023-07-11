public enum GameState { game, menu, pause };

public enum PickupType { ball_fast, ball_multi, ball_small, ghost, life, paddle_thin, paddle_wide, safetyNet };

[System.Serializable]
public struct SettingsFile
{
	public float volume;
	public float speedMouse;
	public float speedKeyboard;
}