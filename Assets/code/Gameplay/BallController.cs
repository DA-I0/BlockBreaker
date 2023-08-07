using UnityEngine;

public class BallController : MonoBehaviour
{
	#region Variables
	private static float MinTravelDistance = 0.00005f;
	private static float DebugRayLength = 1f;
	private static float FlatAngleFixValue = 4f;
	private static int MaxReleaseAngle = 60;
	private static float BounceAnimationSpeed = 0.35f;

	[SerializeField] private float _defaultSpeed = 5f;
	[SerializeField] private float _multiBallSpawnAngle = 30f;

	// In-game used variables
	private float _speedMultiplier = 1f;

	private bool _selectingRotation = false;
	private bool _increasingRotation = true;
	private int _startingRotation = 0;

	private bool _bounced = false;
	private Vector3 _lastPosition = Vector3.zero;

	// References
	[SerializeField] private Transform _arrow;
	private TrailRenderer _boostEffect;
	private Transform _paddle;
	private Animator _animator;

	private AudioController _audio;
	private Gameplay _game;
	#endregion

	#region Methods (public)
	public void ResetBall(bool isFullReset)
	{
		if (gameObject.name != "ball")
		{
			PlayBall();
			return;
		}

		if (isFullReset)
		{
			ChangeSpeed(1f);
			ChangeSize(1f);
		}

		_startingRotation = 0;

		transform.SetParent(_paddle);
		_selectingRotation = false;
		_arrow.gameObject.SetActive(false);

		UpdateBallPosition();
		_lastPosition = transform.localPosition;
		_animator.Play("Base Layer.ball_rolling", 0, _speedMultiplier);
		_animator.StartPlayback();
	}

	public void PlayBall()
	{
		_selectingRotation = false;

		Vector3 newRotation = new Vector3(0f, 0f, _startingRotation);
		transform.Rotate(newRotation);

		transform.SetParent(_game.transform);
		_animator.Play("Base Layer.ball_rolling", 0, _speedMultiplier);
		_animator.StopPlayback();
	}

	public void RotateBall(float turnAngle)
	{
		float newAngle = transform.rotation.z + turnAngle;

		if (IsAngleFlat(newAngle))
		{
			newAngle += (turnAngle < 0) ? FlatAngleFixValue : -FlatAngleFixValue;
		}

		Vector3 newRotation = new Vector3(0f, 0f, newAngle);
		transform.Rotate(newRotation);
	}

	public void ChangeSpeed(float multiplier)
	{
		_speedMultiplier = multiplier;
		_boostEffect.emitting = multiplier > 1f;
	}

	public void ChangeSize(float multiplier)
	{
		transform.localScale = new Vector3(multiplier, multiplier, multiplier);
		float arrowScale = (multiplier == 1) ? 1 : (1 / multiplier);
		_arrow.localScale = new Vector3(arrowScale, arrowScale, arrowScale);
		_boostEffect.widthMultiplier = multiplier;
	}

	public void SpawnCopies()
	{
		InstantiateBall(1);
		InstantiateBall(-1);
	}

	public float GetBallSpeed()
	{
		return _speedMultiplier;
	}

	public float GetBallSize()
	{
		return transform.localScale.x;
	}
	#endregion

	#region Methods (private)
	private void Awake()
	{
		_boostEffect = gameObject.GetComponent<TrailRenderer>();
		_audio = GameObject.Find("_system").GetComponent<AudioController>();
		_game = GameObject.Find("_system").GetComponent<Gameplay>();
		_paddle = GameObject.Find("paddle").transform;
		_animator = gameObject.GetComponent<Animator>();
		ResetBall(true);
	}

	private void Update()
	{
		if (transform.parent == _paddle)
		{
			BallControls();
		}
	}

	private void FixedUpdate()
	{
		_bounced = false;
		UpdateBallPosition();
	}

	private void BallControls()
	{
		if (_game.Mode != GameState.game)
		{
			return;
		}

		if (Input.GetButtonDown("Fire"))
		{
			if (_selectingRotation == false)
			{
				_paddle.GetComponent<PaddleControls>().BlockPaddleMovement(true);
				_selectingRotation = true;
				BallRotationSelect();
			}
			else
			{
				_paddle.GetComponent<PaddleControls>().BlockPaddleMovement(false);
				PlayBall();
			}

			_arrow.gameObject.SetActive(_selectingRotation);
		}
	}

	// NOTE: Ball collision system isn't perfect and can be a bit wonky when interacting
	// with other elements (mainly moving paddle). Improvements needed.
	private void OnCollisionEnter2D(Collision2D collision)
	{
		_audio.PlaySound(1);
		Bounce(collision);
	}

	private void UpdateBallPosition()
	{
		if (transform.parent == _paddle)
		{
			Vector3 newPosition = Vector3.zero;
			newPosition.y += gameObject.GetComponent<CircleCollider2D>().radius * 2;
			transform.localPosition = newPosition;
			transform.rotation = Quaternion.Euler(Vector3.zero);
			gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			return;
		}

		if (transform.parent == _game.transform)
		{
			Vector3 newDirection = Vector3.up * _defaultSpeed * _speedMultiplier * Time.deltaTime;
			transform.Translate(newDirection);

			EmergencyBounce();
			_lastPosition = transform.localPosition;
			return;
		}

		transform.localPosition = Vector3.zero;
	}

	private void Bounce(Collision2D collision)
	{
		if (_bounced)
		{
			return;
		}

		_animator.Play("Base Layer.ball_bounce", 0, BounceAnimationSpeed);

		Debug.DrawRay(collision.GetContact(0).point, transform.up * -DebugRayLength, Color.green, 10f);

		Vector2 reflectNormal = Vector2.Reflect(transform.up, collision.GetContact(0).normal);
		Vector2 temp = new Vector2(transform.up.x, transform.up.y);

		float toRotate = Vector2.SignedAngle(temp, reflectNormal);
		RotateBall(toRotate);

		Debug.DrawRay(collision.GetContact(0).point, reflectNormal * DebugRayLength, Color.red, 10f);

		if (collision.gameObject.GetComponent<Block>() != null)
		{
			collision.gameObject.SendMessage("Damage", 1);
		}

		_bounced = true;
	}

	private bool IsAngleFlat(float currentAngle)
	{
		float angleLean = currentAngle % 90;

		return (Mathf.Abs(angleLean) <= 92 && Mathf.Abs(angleLean) >= 88 ||
		Mathf.Abs(angleLean) <= 2 && Mathf.Abs(angleLean) >= -2);
	}

	private void BallRotationSelect()
	{
		if (_selectingRotation)
		{
			if (_startingRotation >= MaxReleaseAngle || _startingRotation <= -MaxReleaseAngle)
			{
				_increasingRotation = !_increasingRotation;
			}

			_startingRotation += _increasingRotation ? 1 : -1;

			Vector3 newRotation = new Vector3(0f, 0f, _startingRotation);

			_arrow.rotation = Quaternion.Euler(newRotation);
			Invoke("BallRotationSelect", 0.01f);
		}
	}

	private void EmergencyBounce()
	{
		if (IsBallStuck())
		{
			RotateBall(180f);
			Debug.Log("emergency bounce!");
		}
	}

	private void InstantiateBall(int spawnDirection)
	{
		GameObject newBall = Instantiate(gameObject, transform.position, Quaternion.identity);
		newBall.transform.Rotate(
			new Vector3(
				0f,
				0f,
				transform.eulerAngles.z + (_multiBallSpawnAngle * spawnDirection)
			)
		);

		newBall.name = "ball";
		newBall.GetComponent<BallController>().ChangeSpeed(_speedMultiplier);
		newBall.GetComponent<BallController>().ChangeSize(transform.localScale.x);
	}

	private bool IsBallStuck()
	{
		if (Mathf.Abs(_lastPosition.x - transform.localPosition.x) < MinTravelDistance)
		{
			return true;
		}

		if (Mathf.Abs(_lastPosition.y - transform.localPosition.y) < MinTravelDistance)
		{
			return true;
		}

		return false;
	}
	#endregion
}
