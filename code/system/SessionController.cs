using Godot;

public partial class SessionController : Node
{
	private int _currentPaddle = 0;
	private int _currentDifficulty = 1;
	private int _currentLevel = -1;

	public Difficulty difficulty;
	public GameData gameData;
	public AudioController musicController;
	public AudioController audioController;
	public FileOperations fileOperations;
	public Score gameScore;
	// public Gameplay gameState;
	public Health health;
	public LevelManager levelManager;
	// public PaddleControls paddle;
	// public Settings settings;
	public Node gameElements;

	public Paddle paddle;

	public event Notification LastBallLost; // can't be here, gotta move it to a single place

	public Difficulty SelectedDifficulty
	{
		get { return gameData.Difficulties[_currentDifficulty]; }
	}

	public Godot.Collections.Array<Node> Balls
	{
		get { return gameElements.GetChildren(); }
	}

	public override void _Ready()
	{
		gameData = new GameData(this, string.Empty, string.Empty);
		musicController = GetNode("../MusicController") as AudioController;
		audioController = GetNode("../AudioController") as AudioController;
		fileOperations = new FileOperations(this);
		gameScore = GetNode("../Score") as Score;
		gameElements = GetNode("../GameElements");
		health = GetNode("../Health") as Health;
		// gameState = gameObject.GetComponent<Gameplay>();
		levelManager = GetNode("../LevelManager") as LevelManager;
		// settings = gameObject.GetComponent<Settings>();

		Start();
	}

	public void NotifyOfDeath()
	{
		LastBallLost?.Invoke();
	}

	private void Start()
	{
		levelManager.SceneChanged += SetupGameElements;
		gameData.SetupData();
	}

	private void SetupGameElements()
	{
		if (paddle != null)
		{
			return;
		}

		paddle = (Paddle)ResourceLoader.Load<PackedScene>("res://prefabs/paddles/paddle_01.tscn").Instantiate();
		GetParent().AddChild(paddle);
		gameElements.AddChild((Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate());
	}

	public void SelectLevel(int levelIndex)
	{
		_currentLevel = levelIndex;
		levelManager.LoadGameScene(gameData.Levels[_currentLevel]);
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
}
