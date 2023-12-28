using Godot;

public enum GameState { gameplay, menu, pause, gameOver, gameWin }

public partial class SessionController : Node
{
	private GameState _gameState = GameState.menu;
	private int _currentPaddle;
	private int _currentDifficulty;
	private int _currentLevel;

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
	private int _selectedSkillIndex = 0;

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

	public bool IsCustomDifficultySelected
	{
		get { return _currentDifficulty > (gameData.DefaultDifficultyCount - 1); }
	}

	public string SelectedPaddle
	{
		get { return $"res://prefabs/paddles/paddle_{_currentPaddle}.tscn"; }
	}

	public int SelectedPaddleIndex
	{
		get { return _currentPaddle; }
	}

	public Godot.Collections.Array<Node> Balls
	{
		get { return gameElements.GetChild(0).GetChildren(); }
	}

	public Skill SelectedSkill
	{
		get { return gameData.Skills[_selectedSkillIndex]; }
	}

	public override void _Ready()
	{
		SetupReferences();
		SetupInitialValues();
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
		health.GameOver += GameOver;
	}

	private void SetupInitialValues()
	{
		gameData.SetupData();
		localization.UpdateUILocalization();
		levelManager.LoadMenuScene();
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

		SelectedSkill.Setup(this);
		SelectedSkill.SkillReady += EnableSkill;
		SelectedSkill.SkillUsed += UseSkillNotification;

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
			ChangeGameState(GameState.gameWin);
		}
	}

	private void ResetSession()
	{
		SelectedSkill.SkillReady -= EnableSkill;
		SelectedSkill.SkillUsed -= UseSkillNotification;

		_currentPaddle = 1;
		_currentDifficulty = 1;
		_currentLevel = -1;
		_selectedSkillIndex = 0;

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
		UseSkill(@event);
		SetActiveInputType(@event);
	}

	public void SetPaddle(int number)
	{
		_currentPaddle = number;
	}

	public void SetDifficulty(int index)
	{
		_currentDifficulty = index;
	}

	public void SetSkill(int index)
	{
		_selectedSkillIndex = index;
	}

	private void UseSkill(InputEvent @event)
	{
		if (_gameState == GameState.gameplay && @event.IsActionReleased("game_skill"))
		{
			SelectedSkill.Activate();
		}
	}

	private void SetActiveInputType(InputEvent @event)
	{
		if (@event.AsText().Contains("Joypad"))
		{
			settings.ActiveInputType = InputType.Joypad;
			return;
		}

		if (@event.AsText().Contains("Mouse"))
		{
			settings.ActiveInputType = InputType.Mouse;
			return;
		}

		settings.ActiveInputType = InputType.Keyboard;
	}

	private void UseSkillNotification()
	{
		SkillUsed?.Invoke();
	}

	private void EnableSkill()
	{
		audioController.PlayAudio(4);
		SkillReady?.Invoke();
	}

	private void GameOver()
	{
		ChangeGameState(GameState.gameOver);
	}
}
