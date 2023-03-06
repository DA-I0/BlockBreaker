using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControler : MonoBehaviour
{
	#region Variables
	private static float DebugRayLength = 1f;
	private static float FlatAngleFixValue = 4f;
	private static int MaxStartingAngle = 60;

	[SerializeField] private float _defaultSpeed = 5f;
	[SerializeField] private float _multiBallSpawnAngle = 30f;

	private float _speedMultiplier = 1f;

	[SerializeField] private Transform _arrow;
	private TrailRenderer _boostEffect;
	private Transform _paddle;
	private Animator _animator;

	private Transform _systemObject;
	private GameState _game;

	private bool _rotationSelect = false;
	private bool _rotationIncrease = true;
	private int _startingRotation = 0;

	private bool _bounced = false;
	private Vector3 _lastPosition = Vector3.zero;
	#endregion

	#region Methods (public)
	public void ResetBall()
	{
		if (gameObject.name != "ball")
		{
			PlayBall();
			return;
		}

		ChangeSpeed(1f);
		ChangeSize(1f);

		_startingRotation = 0;

		transform.SetParent(_paddle);
		_arrow.gameObject.SetActive(_rotationSelect);

		_lastPosition = transform.localPosition;
		UpdateBallPosition();
		_animator.Play("Base Layer.ball_rolling", 0, 0);
		_animator.StartPlayback();
	}

	public void PlayBall()
	{
		_rotationSelect = false;

		Vector3 newRotation = new Vector3(0f, 0f, _startingRotation);
		transform.Rotate(newRotation);

		transform.SetParent(_systemObject);
		_animator.StopPlayback();
	}

	public void ChangeSpeed(float multiplier)
	{
		_speedMultiplier = multiplier;

		_boostEffect.emitting = multiplier > 1f;
		_animator.speed = multiplier;
	}

	public void ChangeSize(float multiplier)
	{
		transform.localScale = new Vector3(multiplier, multiplier, multiplier);
		_boostEffect.widthMultiplier = multiplier;
	}

	public void SpawnCopies()
	{
		InstantiateBall(1);
		InstantiateBall(-1);
	}
	#endregion

	#region Methods (private)
	private void Awake()
	{
		_boostEffect = gameObject.GetComponent<TrailRenderer>();
		_systemObject = GameObject.Find("_system").transform;
		_paddle = GameObject.Find("paddle").transform;
		_animator = gameObject.GetComponent<Animator>();
		ResetBall();
	}

	private void Start()
	{
		_game = GameObject.Find("_system").GetComponent<GameState>();
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
		if (Input.GetButtonDown("Fire"))
		{
			if (_rotationSelect == false)
			{
				_rotationSelect = true;
				RotationSelect();
			}
			else
			{
				PlayBall();
			}

			_arrow.gameObject.SetActive(_rotationSelect);
		}
	}

	// Ball collision system isn't perfect and can be a bit wonky when interacting
	// with other elements (mainly moving paddle). Improvements needed.
	private void OnCollisionEnter2D(Collision2D collision)
	{
		_systemObject.GetComponent<GameState>().PlaySound(1);
		Bounce(collision);

		if (collision.gameObject.name != "map_background")
		{
			Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), false);
	}

	private void UpdateBallPosition()
	{
		if (transform.parent == _systemObject)
		{
			transform.Translate(Vector3.up * _defaultSpeed * _speedMultiplier * Time.deltaTime);

			EmergencyBounce();
			_lastPosition = transform.localPosition;
			return;
		}

		if (transform.parent == _paddle)
		{
			Vector3 newPosition = _paddle.position;
			newPosition.y += gameObject.GetComponent<CircleCollider2D>().radius * 2;
			transform.position = newPosition;
			transform.rotation = Quaternion.Euler(Vector3.zero);
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

		_animator.Play("Base Layer.ball_bounce", 0, 0.25f);

		Debug.DrawRay(collision.GetContact(0).point, transform.up * -DebugRayLength, Color.green, 10f);

		Vector2 reflectNormal = Vector2.Reflect(transform.up, collision.GetContact(0).normal);
		Vector2 temp = new Vector2(transform.up.x, transform.up.y);

		float toRotate = Vector2.SignedAngle(temp, reflectNormal);
		Vector3 newRotation = new Vector3(0f, 0f, AngleCheck(transform.rotation.z, toRotate));
		transform.Rotate(newRotation);

		Debug.DrawRay(collision.GetContact(0).point, reflectNormal * DebugRayLength, Color.red, 10f);

		if (collision.gameObject.GetComponent<Block>() != null)
		{
			collision.gameObject.SendMessage("Damage", 1);
		}

		_bounced = true;
	}

	private float AngleCheck(float currentAngle, float turnAngle)
	{
		float newAngle = currentAngle + turnAngle;

		if (IsAngleFlat(newAngle))
		{
			newAngle += (turnAngle < 0) ? FlatAngleFixValue : -FlatAngleFixValue;
		}

		return newAngle;
	}

	private bool IsAngleFlat(float currentAngle)
	{
		float angleLean = currentAngle % 90;
		Debug.Log(currentAngle + " : " + angleLean);

		return (Mathf.Abs(angleLean) <= 92 && Mathf.Abs(angleLean) >= 88 ||
		Mathf.Abs(angleLean) <= 2 && Mathf.Abs(angleLean) >= -2);
	}

	private void RotationSelect()
	{
		if (_rotationSelect)
		{
			if (_startingRotation >= MaxStartingAngle || _startingRotation <= -MaxStartingAngle)
			{
				_rotationIncrease = !_rotationIncrease;
			}

			_startingRotation += _rotationIncrease ? 1 : -1;

			Vector3 newRotation = new Vector3(0f, 0f, _startingRotation);

			_arrow.rotation = Quaternion.Euler(newRotation);
			Invoke("RotationSelect", 0.01f);
		}
	}

	private void EmergencyBounce()
	{
		if (_lastPosition.x == transform.localPosition.x || _lastPosition.y == transform.localPosition.y)
		{
			Vector3 newRotation = new Vector3(0f, 0f, AngleCheck(transform.rotation.z, 180f));
			transform.Rotate(newRotation);
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
				newBall.transform.rotation.z + (_multiBallSpawnAngle * spawnDirection)
			)
		);
		newBall.name = "ball";
	}
	#endregion
}
