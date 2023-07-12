using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
	[SerializeField] private GameObject _door;
	[SerializeField] private GameObject _arrow;
	private BoxCollider2D _exitTrigger;

	private void Awake()
	{
		_exitTrigger = gameObject.GetComponent<BoxCollider2D>();
	}

	private void Start()
	{
		LevelClear(false);
	}

	public void LevelClear(bool isActive)
	{
		_door.SetActive(!isActive);
		_arrow.SetActive(isActive);
		_exitTrigger.enabled = isActive;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball")
		{
			GameObject.Find("_system").GetComponent<GameScore>().AddBonusScore();
			gameObject.GetComponent<LevelManager>().NextLevel();
		}
	}
}
