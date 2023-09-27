using Godot;

public partial class SessionController : Node
{
	private int _currentPaddle = 0;
	private int _currentDifficulty = 1;
	private int _currentLevel = -1;

	public GameData gameData;
	[Export] public AudioController musicController;
	[Export] public AudioController audioController;
	public FileOperations fileOperations;
	[Export] public Score gameScore;
	[Export] public Health health;
	[Export] public LevelManager levelManager;
	public Settings settings;
	[Export] public Node gameElements;

	public Paddle paddle;

	public event Notification LastBallLost; // can't be here, gotta move it to a single place

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
		gameData = new GameData(this);
		fileOperations = new FileOperations(this);
		settings = new Settings(this);

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
		gameElements.AddChild(paddle);
		gameElements.GetChild(0).AddChild((Ball)ResourceLoader.Load<PackedScene>("res://prefabs/ball.tscn").Instantiate());
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
