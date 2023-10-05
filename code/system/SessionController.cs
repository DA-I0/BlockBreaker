using Godot;

public enum GameState { gameplay, menu, pause }

public partial class SessionController : Node
{
	private GameState _gameState = GameState.menu;
	private int _currentPaddle = -1;
	private int _currentDifficulty = -1;
	private int _currentLevel = -1;

	public GameData gameData;
	public FileOperations fileOperations;
	public Settings settings;
	public LocalizationController localization;
	[Export] public AudioController musicController;
	[Export] public AudioController audioController;
	[Export] public Score gameScore;
	[Export] public Health health;
	[Export] public LevelManager levelManager;
	[Export] public Node gameElements;

	public Paddle paddle;
	[Export] private Timer _skillTimer;

	public event Notification LastBallLost; // can't be here, gotta move it to a single place
	public event Notification GameStateChanged;
	public event Notification GameSetup;
	public event Notification SkillReady;
	public event Notification SkillUsed;

	public GameState CurrentGameState
	{
		get { return _gameState; }
	}

	public Difficulty SelectedDifficulty
	{
		get { return gameData.Difficulties[_currentDifficulty]; }
	}

	public string SelectedPaddle
	{
		get { return $"res://prefabs/paddles/paddle_{_currentPaddle}.tscn"; }
	}

	public Godot.Collections.Array<Node> Balls
	{
		get { return gameElements.GetChild(0).GetChildren(); }
	}

	public override void _Ready()
	{
		SetupReferences();
		gameData.SetupData();
		localization.UpdateUILocalization();
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
		localization = new LocalizationController(this);

		levelManager.ResetSession += ResetSession;
		levelManager.SceneChanged += SetupGameElements;
	}

	private void SetupGameElements()
	{
		if (paddle != null)
		{
			return;
		}

		paddle = (Paddle)ResourceLoader.Load<PackedScene>(SelectedPaddle).Instantiate();
		gameElements.AddChild(paddle);

		Ball ball = (Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate();
		ball.SetInitialValues(this);
		gameElements.GetChild(0).AddChild(ball);

		_skillTimer.Start();
		GameSetup?.Invoke();
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
		_currentPaddle = -1;
		_currentDifficulty = -1;
		_currentLevel = -1;
		_skillTimer.Stop();
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
		Shake(@event);

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

	public void SetPaddle(int number)
	{
		_currentPaddle = number;
	}

	public void SetDifficulty(int index)
	{
		_currentDifficulty = index;
	}

	private void Shake(InputEvent @event)
	{
		if (_gameState == GameState.gameplay && @event.IsActionReleased("game_skill"))
		{
			if (_skillTimer.TimeLeft <= 0)
			{
				paddle.SetPaddleState(PaddleState.confused, 3f);

				for (int i = 0; i < Balls.Count; i++)
				{
					float angleChange = GD.RandRange(5, 15);
					int direction = GD.RandRange(-1, 1) < 0 ? -1 : 1;
					((Ball)Balls[i]).ChangeRotation(angleChange * direction);
				}

				audioController.PlayAudio(3);
				_skillTimer.Start();
				SkillUsed?.Invoke();
			}
		}
	}

	private void EnableSkill()
	{
		audioController.PlayAudio(4);
		SkillReady?.Invoke();
	}
}
