using UnityEngine;

public class HazardWhirpool : MonoBehaviour
{
	[SerializeField] private int _deactivationTime = 10;
	[SerializeField] private Color _deactivatedColor;

	private bool _active = true;
	private int _timeLeft = -1;

	private GameObject _ball;

	private SpriteRenderer _spriteRenderer;
	private Animator _animator;

	private void Awake()
	{
		_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		_animator = gameObject.GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (_active && collider.gameObject.tag == "ball" && _ball == null)
		{
			Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), true);

			_ball = collider.gameObject;
			_ball.transform.SetParent(transform);

			Vector3 newRotation = new Vector3(0f, 0f, Random.Range(-180.0f, 180.0f));
			newRotation.z = (newRotation.z == 90f || newRotation.z == -90f) ? newRotation.z + 5f : newRotation.z;
			_ball.transform.transform.Rotate(newRotation);

			collider.transform.position = transform.position;
			Invoke("ReleaseBall", 1f);
		}
	}

	private void ReleaseBall()
	{
		_ball.transform.SetParent(GameObject.Find("_system").transform);
		_ball = null;
		StartTimer();
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "ball")
		{
			Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), false);
		}
	}

	private void StartTimer()
	{
		_timeLeft = _deactivationTime;
		ToggleHazard();
		Invoke("Timer", 1f);
	}

	private void ToggleHazard()
	{
		_active = _timeLeft <= 0;
		_spriteRenderer.color = _active ? Color.white : _deactivatedColor;
		_animator.speed = _active ? 1f : 0f;
	}

	private void Timer()
	{
		_timeLeft--;
		ToggleHazard();

		if (_timeLeft > 0)
		{
			Invoke("Timer", 1f);
			return;
		}
	}
}
