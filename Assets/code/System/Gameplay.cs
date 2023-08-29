using UnityEngine;

public class Gameplay : MonoBehaviour
{
	#region Variables
	public static int BonusPoints = 10;
	public static GameObject _highlander;

	private GameState _gameState = GameState.game;

	[SerializeField] private int _maxLives = 5;
	[SerializeField] private int _startingLives = 3;

	private int _lives = 0;
	private int _blocksLeft;

	// References
	private AudioController _audio;
	private GameScore _gameScore;
	private Transform _blocks;
	private Transform _barriers;
	private Transform _safetyNet;
	private PaddleControls _paddle;
	private LevelExit _levelExit;
	private Settings _settings;
	#endregion

	#region Properties
	public GameState Mode
	{
		get { return _gameState; }
	}

	public int Lives
	{
		get { return _lives; }
	}

	public int BlocksLeft
	{
		get { return _blocksLeft; }
	}
	#endregion

	#region Methods (public)
	public void FindGameElements()
	{
		_paddle.gameObject.SetActive(true);
		_blocks = GameObject.Find("blocks").transform;
		_barriers = GameObject.Find("barriers_breakable").transform;
		_levelExit = GameObject.Find("level_exit").GetComponent<LevelExit>();

		SetupLevel();
	}

	public void Cleanup(bool isFullReset)
	{
		_blocks = null;
		_barriers = null;

		if (isFullReset)
		{
			_gameScore.Cleanup();
			_lives = _startingLives;
			CleanBalls(true);
		}

		if (_paddle != null)
		{
			_paddle.ResetPaddle();
			_paddle.gameObject.SetActive(false);
		}

		DisplaySafetyNet(false);
		ChangeGameState("menu");
		_audio.ChangeSong(0);
	}

	public void SetupLevel()
	{
		CleanBalls(false);
		_paddle.RecenterPaddle();

		foreach (Transform block in _blocks)
		{
			block.GetComponent<Block>().ResetBlock();
		}

		foreach (Transform barrier in _barriers)
		{
			barrier.GetComponent<Block>().ResetBlock();
		}

		_blocksLeft = _blocks.childCount;

		BroadcastMessage("UpdateScore");
		BroadcastMessage("UpdateLives");
		ChangeGameState("game");
		_audio.ChangeSong(1);
	}

	public void ChangeProgress(int points)
	{
		_gameScore.ChangeScore(points);
		_blocksLeft--;

		if (_blocksLeft <= 0)
		{
			_gameScore.InvokeExitTimer();
			_levelExit.LevelClear(true);
		}
	}

	public void ChangeLives(int amount)
	{
		_lives += amount;

		if (_lives > _maxLives)
		{
			_lives = _maxLives;
			_gameScore.ChangeScore(BonusPoints);
		}

		if (amount < 0)
		{
			_audio.PlaySound(0);
			_paddle.ResetPaddle();
			GameObject.Find("ball").GetComponent<BallController>().ResetBall(true);
			_gameScore.ResetMultiplier();
		}

		if (_lives < 0)
		{
			gameObject.GetComponent<LevelManager>().LoadMainMenu();
		}

		BroadcastMessage("UpdateLives");
	}

	public void DisplaySafetyNet(bool activate)
	{
		foreach (Transform block in _safetyNet)
		{
			if (activate)
			{
				block.GetComponent<Block>().ResetBlock();
			}
			else
			{
				block.gameObject.SetActive(false);
			}
		}
	}

	public void ChangeGameState(string newState)
	{
		switch (newState)
		{
			case "game":
				_gameState = GameState.game;
				Time.timeScale = 1f;
				break;

			case "pause":
				_gameState = GameState.pause;
				Time.timeScale = 0f;
				break;

			default:
				_gameState = GameState.menu;
				break;
		}

		BroadcastMessage("TogglePauseMenu");
	}
	#endregion

	#region Methods (private)
	private void Awake()
	{
		if (_highlander != null && _highlander != gameObject)
		{
			Destroy(gameObject);
			return;
		}

		_highlander = gameObject;
		_settings = gameObject.GetComponent<Settings>();
		_audio = gameObject.GetComponent<AudioController>();
		_gameScore = gameObject.GetComponent<GameScore>();
		_safetyNet = GameObject.Find("safetyNet").transform;
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(Camera.main.gameObject);

		_paddle = GameObject.Find("paddle").GetComponent<PaddleControls>();
		Cleanup(false);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Restart"))
		{
			ChangeLives(-1);
		}

		if (_gameState != GameState.menu && Input.GetButtonDown("Cancel"))
		{
			if (_gameState == GameState.game)
			{
				ChangeGameState("pause");
			}
			else
			{
				ChangeGameState("game");
			}

			BroadcastMessage("TogglePauseMenu");
		}
	}

	private void CleanBalls(bool isFullReset)
	{
		GameObject[] ballCopies = GameObject.FindGameObjectsWithTag("ball");

		for (int i = 0; i < ballCopies.Length; i++)
		{
			if (i == 0)
			{
				ballCopies[i].GetComponent<BallController>().ResetBall(isFullReset);
				continue;
			}

			Destroy(ballCopies[i]);
		}
	}
	#endregion
}
