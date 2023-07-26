using UnityEngine;

public class Resetzone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		switch (collider.gameObject.tag)
		{
			case "ball":
				collider.gameObject.GetComponent<BallControler>().ResetBall(false);
				break;

			case "Player":
				collider.gameObject.GetComponent<PaddleControls>().ResetPaddle();
				break;

			default:
				Destroy(collider.gameObject);
				break;
		}
	}
}
