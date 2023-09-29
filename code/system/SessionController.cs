using Godot;

public enum GameState { gameplay, menu, pause }

public partial class SessionController : Node
{
	private GameState _gameState = GameState.menu;
	private int _currentPaddle = 0;
	private int _currentDifficulty = 1;
	private int _currentLevel = -1;

	public GameData gameData;
	public FileOperations fileOperations;
	public Settings settings;
	[Export] public AudioController musicController;
	[Export] public AudioController audioController;
	[Export] public Score gameScore;
	[Export] public Health health;
	[Export] public LevelManager levelManager;
	[Export] public Node gameElements;

	public Paddle paddle;

	public event Notification LastBallLost; // can't be here, gotta move it to a single place
	public event Notification GameStateChanged;

	public GameState CurrentGameState
	{
		get { return _gameState; }
	}

	public Difficulty SelectedDifficulty
	{
		get { return gameData.Difficulties[_currentDifficulty]; }
	}

	public Godot.Collections.Array<Node> Balls
	{
		get { return gameElements.GetChild(0).GetChildren(); }
	}

	public override void _Ready()
	{
		SetupReferences();
		gameData.SetupData();
	}

	public void NotifyOfDeath()
	{
		LastBallLost?.Invoke();
	}

	private void SetupReferences()
	{
		gameData = new GameData(this);
		fileOperations = new FileOperations(this);
		settings = new Settings(this);

		levelManager.ResetSession += ResetSession;
		levelManager.SceneChanged += SetupGameElements;
	}

	private void SetupGameElements()
	{
		if (paddle != null)
		{
			return;
		}

		paddle = (Paddle)ResourceLoader.Load<PackedScene>("res://prefabs/paddles/paddle_01.tscn").Instantiate();
		gameElements.AddChild(paddle);

		Ball startingBall = (Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate();
		startingBall.Position = paddle.Position;
		gameElements.GetChild(0).AddChild(startingBall);
	}

	public void SelectLevel(int levelIndex)
	{
		_currentLevel = levelIndex;
		levelManager.LoadGameScene(gameData.Levels[_currentLevel]);
		ChangeGameState(GameState.gameplay);
	}

	public void AdvanceCurrentLevel()
	{
		_currentLevel++;
		if (gameData.Levels.Count > _currentLevel)
		{
			levelManager.LoadGameScene(gameData.Levels[_currentLevel]);
		}
		else
		{
			_currentLevel = -1;
			levelManager.LoadMenuScene();
		}
	}

	private void ResetSession()
	{
		_currentLevel = -1;
		ChangeGameState(GameState.menu);
	}

	public void ChangeGameState(GameState state)
	{
		_gameState = state;
		GetTree().Paused = (_gameState == GameState.pause);
		Input.MouseMode = (_gameState == GameState.gameplay) ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
		GameStateChanged?.Invoke();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.AsText().Contains("Joypad"))
		{
			settings.ActiveController = InputType.gamepad;
			return;
		}

		if (@event.AsText().Contains("Mouse"))
		{
			settings.ActiveController = InputType.mouse;
			return;
		}

		settings.ActiveController = InputType.keyboard;
	}
}
