using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField] private PickupType _type;
	[SerializeField] private int _pointValue = 2;
	[SerializeField] private float _defaultSpeed = 1f;

	private void FixedUpdate()
	{
		transform.Translate(Vector3.down * _defaultSpeed * Time.deltaTime);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.name == "paddle")
		{
			switch (_type)
			{
				case PickupType.ball_fast:
					foreach (GameObject ball in FindAllBalls())
						ball.GetComponent<BallControler>().ChangeSpeed(1.5f);
					break;

				case PickupType.ball_multi:
					foreach (GameObject ball in FindAllBalls())
						ball.GetComponent<BallControler>().SpawnCopies();
					break;

				case PickupType.ball_small:
					foreach (GameObject ball in FindAllBalls())
						ball.GetComponent<BallControler>().ChangeSize(0.5f);
					break;

				case PickupType.ghost:
					GameObject.Find("_system").GetComponent<Gameplay>().ChangeLives(-1);
					break;

				case PickupType.life:
					GameObject.Find("_system").GetComponent<Gameplay>().ChangeLives(1);
					break;

				case PickupType.paddle_thin:
					collision.gameObject.GetComponent<PaddleControls>().ChangePaddleSize(-1);
					break;

				case PickupType.paddle_wide:
					collision.gameObject.GetComponent<PaddleControls>().ChangePaddleSize(1);
					break;

				case PickupType.safetyNet:
					GameObject.Find("_system").GetComponent<Gameplay>().ToggleSafetyNet(true);
					break;

				default:
					break;
			}

			GameObject.Find("_system").GetComponent<GameScore>().ChangeScore(_pointValue, false);
			Destroy(gameObject);
		}
	}

	private GameObject[] FindAllBalls()
	{
		return GameObject.FindGameObjectsWithTag("ball");
	}

	private void Explode()
	{
		Destroy(gameObject);
	}
}
