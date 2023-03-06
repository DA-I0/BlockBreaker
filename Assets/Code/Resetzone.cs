using UnityEngine;

public class Resetzone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball")
		{
			collider.gameObject.GetComponent<BallControler>().ResetBall();
		}
		else
		{
			Destroy(collider.gameObject);
		}
	}
}
