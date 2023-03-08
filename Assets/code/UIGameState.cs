using UnityEngine;
using UnityEngine.UI;

public class UIGameState : MonoBehaviour
{
	[SerializeField] private GameObject _gameStateBar;
	[SerializeField] private Text _score;
	[SerializeField] private Transform _lives;
	[SerializeField] private Text _info;

	private GameState _system;

	private void Awake()
	{
		_system = GameObject.Find("_system").GetComponent<GameState>();
	}

	private void FixedUpdate()
	{
		_info.text = "R - restart\nBlocks remaining: " + _system.BlocksLeft.ToString();
	}

	public void ToggleGameStats(bool hide)
	{
		_gameStateBar.SetActive(hide);
	}

	public void UpdateScore()
	{
		_score.text = "SCORE: " + _system.Score.ToString();
	}

	public void UpdateLives()
	{
		foreach (Transform live in _lives)
		{
			live.gameObject.SetActive(live.GetSiblingIndex() < _system.Lives);
		}
	}
}
