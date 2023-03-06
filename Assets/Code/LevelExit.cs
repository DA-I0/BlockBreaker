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

	public void LevelClear(bool active)
	{
		_door.SetActive(!active);
		_arrow.SetActive(active);
		_exitTrigger.enabled = active;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball")
		{
			gameObject.GetComponent<LevelManager>().NextLevel();
		}
	}
}
