using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameState : MonoBehaviour
{
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private GameObject _gameStateBar;
	[SerializeField] private TMP_Text _score;
	[SerializeField] private Transform _lives;
	[SerializeField] private TMP_Text _bonusTime;
	[SerializeField] private GameObject _proceedText;

	private Gameplay _system;
	private GameScore _gameScore;

	public void ToggleGameStats(bool hide)
	{
		_gameStateBar.SetActive(hide);
	}

	public void TogglePauseMenu()
	{
		if (_system.Mode == GameState.pause)
		{
			_pauseMenu.SetActive(true);
		}
		else
		{
			_pauseMenu.SetActive(false);
		}

		ToggleCursor();
	}

	public void UpdateScore()
	{
		_score.text = $"SCORE: {_gameScore.Score.ToString()}";

		if (_gameScore.Multiplier > 1)
		{
			_score.text += $" (x{_gameScore.Multiplier.ToString()})";
		}
	}

	public void UpdateLives()
	{
		foreach (Transform live in _lives)
		{
			live.gameObject.SetActive(live.GetSiblingIndex() < _system.Lives);
		}
	}

	public void UpdateTimer()
	{
		_bonusTime.transform.parent.gameObject.SetActive(_gameScore.TimeToExit > 0);
		_bonusTime.text = _gameScore.TimeToExit.ToString();

		_proceedText.SetActive(_gameScore.TimeToExit > -99 && _gameScore.TimeToExit < 1);
	}

	public void ToggleCursor()
	{
		if (_system.Mode == GameState.game)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void Awake()
	{
		_system = GameObject.Find("_system").GetComponent<Gameplay>();
		_gameScore = GameObject.Find("_system").GetComponent<GameScore>();

		_bonusTime.transform.parent.gameObject.SetActive(false);
		_proceedText.SetActive(false);
	}
}
