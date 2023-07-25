using UnityEngine;

public class Gameplay : MonoBehaviour
{
	#region Variables
	public static GameObject _highlander;

	private GameState _gameState = GameState.game;

	[SerializeField] private AudioClip[] _sounds;
	[SerializeField] private int _maxLives = 5;
	[SerializeField] private int _startingLives = 3;

	private int _lives = 0;
	private int _blocksLeft;

	private AudioSource _soundSource;
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
		set { _lives = value; }
	}

	public int BlocksLeft
	{
		get { return _blocksLeft; }
	}
	#endregion

	#region Methods (public)
	public void SetupLevel()
	{
		_paddle.gameObject.SetActive(true);
		_blocks = GameObject.Find("blocks").transform;
		_barriers = GameObject.Find("barriers_breakable").transform;
		_safetyNet = GameObject.Find("safetyNet").transform;
		_levelExit = GameObject.Find("level_exit").GetComponent<LevelExit>();

		StartFreshGame();
	}

	public void Cleanup(bool isFullReset)
	{
		_blocks = null;
		_barriers = null;

		if (isFullReset)
		{
			_gameScore.Cleanup();
			_lives = _startingLives;
			CleanBalls();
		}

		if (_paddle != null)
		{
			_paddle.ResetPaddle();
			_paddle.gameObject.SetActive(false);
		}

		ChangeGameState("menu");
	}

	public void PlaySound(int type)
	{
		_soundSource.PlayOneShot(_sounds[type], _settings.Volume);
	}

	public void StartFreshGame()
	{
		CleanBalls();
		_paddle.ResetPaddle();

		foreach (Transform block in _blocks)
		{
			block.GetComponent<Block>().ResetBlock();
		}

		foreach (Transform barrier in _barriers)
		{
			barrier.GetComponent<Block>().ResetBlock();
		}

		ToggleSafetyNet(false);
		_blocksLeft = _blocks.childCount;

		BroadcastMessage("UpdateScore");
		BroadcastMessage("UpdateLives");
		ChangeGameState("game");
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
		}

		if (amount < 0)
		{
			PlaySound(0);
			_paddle.ResetPaddle();
			GameObject.Find("ball").GetComponent<BallControler>().ResetBall(true);
			_gameScore.ResetMultiplier();
		}

		if (_lives < 0)
		{
			gameObject.GetComponent<LevelManager>().LoadMainMenu();
		}

		BroadcastMessage("UpdateLives");
	}

	public void ToggleSafetyNet(bool isActive)
	{
		foreach (Transform block in _safetyNet)
		{
			if (isActive)
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
		_gameScore = gameObject.GetComponent<GameScore>();
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(Camera.main.gameObject);

		_soundSource = gameObject.GetComponent<AudioSource>();
		_paddle = GameObject.Find("paddle").GetComponent<PaddleControls>();
		Cleanup(false);
	}

	private void CleanBalls()
	{
		GameObject[] ballCopies = GameObject.FindGameObjectsWithTag("ball");

		for (int i = 0; i < ballCopies.Length; i++)
		{
			if (i == 0)
			{
				ballCopies[i].GetComponent<BallControler>().ResetBall(false);
				continue;
			}

			Destroy(ballCopies[i]);
		}
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
	#endregion
}
