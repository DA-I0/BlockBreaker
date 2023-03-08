using UnityEngine;

public class Killzone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			return;
		}

		if (collider.gameObject.tag == "ball")
		{
			GameObject[] balls = GameObject.FindGameObjectsWithTag("ball");

			if (balls.Length > 1)
			{
				Destroy(collider.gameObject);
			}
			else
			{
				GameObject.Find("_system").GetComponent<GameState>().ChangeLives(-1);
			}
		}
		else
		{
			Destroy(collider.gameObject);
		}
	}
}
