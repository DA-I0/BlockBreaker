using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_Whirpool : MonoBehaviour
{
	private GameObject _ball;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball" && _ball == null)
		{
			Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), true);

			_ball = collider.gameObject;
			_ball.transform.SetParent(transform);

			Vector3 newRotation = new Vector3(0f, 0f, Random.Range(-180.0f, 180.0f));
			newRotation.z = (newRotation.z == 90f || newRotation.z == -90f) ? newRotation.z + 5f : newRotation.z;
			_ball.transform.transform.Rotate(newRotation);

			collider.transform.position = transform.position;
			Invoke("FreeBall", 1f);
		}
	}

	private void FreeBall()
	{
		_ball.transform.SetParent(GameObject.Find("_system").transform);
		_ball = null;
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball")
		{
			Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), false);
		}
	}
}
