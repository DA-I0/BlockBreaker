using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
	#region Variables
	public static GameObject _highlander;

	[SerializeField] private AudioClip[] _sounds;
	[SerializeField] private int _maxLives = 5;
	[SerializeField] private int _startingLives = 3;

	private int _score = 0;
	private int _lives = 0;
	private int _blocksLeft;

	private AudioSource _soundSource;
	private Transform _blocks;
	private Transform _barriers;
	private Transform _safetyNet;
	private PaddleControls _paddle;
	private LevelExit _levelExit;
	#endregion

	#region Properties
	public int Score
	{
		get { return _score; }
		set { _score = value; }
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

	public void Cleanup(bool fullReset)
	{
		_blocks = null;
		_barriers = null;

		if (fullReset)
		{
			_score = 0;
			_lives = _startingLives;
			CleanBalls();
		}

		if (_paddle != null)
		{
			_paddle.gameObject.SetActive(false);
		}
	}

	public void PlaySound(int type)
	{
		_soundSource.PlayOneShot(_sounds[type], 0.5f);
	}

	public void StartFreshGame()
	{
		_paddle.ResetPaddle();
		CleanBalls();

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
	}

	public void ChangeProgress(int points)
	{
		ChangeScore(points);
		_blocksLeft--;

		if (_blocksLeft <= 0)
		{
			_levelExit.LevelClear(true);
		}
	}

	public void ChangeScore(int amount)
	{
		_score += amount;

		PlaySound(2);
		BroadcastMessage("UpdateScore");
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
			GameObject.Find("ball").GetComponent<BallControler>().ResetBall();
		}

		if (_lives < 0)
		{
			gameObject.GetComponent<LevelManager>().LoadMainMenu();
		}

		BroadcastMessage("UpdateLives");
	}

	public void ToggleSafetyNet(bool active)
	{
		foreach (Transform block in _safetyNet)
		{
			if (active)
			{
				block.GetComponent<Block>().ResetBlock();
			}
			else
			{
				block.gameObject.SetActive(false);
			}
		}
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
				ballCopies[i].GetComponent<BallControler>().ResetBall();
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

		if (Input.GetButtonDown("Cancel"))
		{
			gameObject.GetComponent<LevelManager>().LoadMainMenu();
		}
	}
	#endregion
}
