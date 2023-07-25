using UnityEngine;

public class LevelExit : MonoBehaviour
{
	[SerializeField] private GameObject _door;
	[SerializeField] private GameObject _arrow;
	private BoxCollider2D _exitTrigger;

	private GameScore _score;

	public void LevelClear(bool isActive)
	{
		_door.SetActive(!isActive);
		_arrow.SetActive(isActive);
		_exitTrigger.enabled = isActive;
	}

	private void Awake()
	{
		_exitTrigger = gameObject.GetComponent<BoxCollider2D>();
		_score = GameObject.Find("_system").GetComponent<GameScore>();
	}

	private void Start()
	{
		LevelClear(false);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball")
		{
			gameObject.GetComponent<LevelManager>().NextLevel();
		}
	}

	private void FixedUpdate()
	{
		if (_exitTrigger.enabled && _score.TimeToExit < 1)
		{
			if (Input.GetButtonDown("Fire"))
			{
				gameObject.GetComponent<LevelManager>().NextLevel();
			}
		}
	}
}
