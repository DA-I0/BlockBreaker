using UnityEngine;

public class PaddleControls : MonoBehaviour
{
	#region Variables
	private static int MinSizeMultiplier = -2;
	private static int MaxSizeMultiplier = 4;

	[SerializeField] private int _sizeMultiplier = 0;

	private bool _freezePaddle = false;

	private SpriteRenderer _sprite;
	private Rigidbody2D _paddleRB;

	private Settings _settings;
	#endregion

	public int SizeMultiplier
	{
		get { return _sizeMultiplier; }
		set { _sizeMultiplier = value; }
	}

	public void ResetPaddle()
	{
		_sizeMultiplier = 0;
		ResizePaddle();
		transform.position = new Vector3(0f, transform.position.y, transform.position.z);
		_freezePaddle = false;
	}

	public void ChangePaddleSize(int multiplier)
	{
		_sizeMultiplier += multiplier;

		if (_sizeMultiplier < MinSizeMultiplier)
		{
			_sizeMultiplier = MinSizeMultiplier;
		}

		if (_sizeMultiplier > MaxSizeMultiplier)
		{
			_sizeMultiplier = MaxSizeMultiplier;
		}

		ResizePaddle();
	}

	public void BlockPaddleMovement(bool blockMovement)
	{
		_freezePaddle = blockMovement;
	}

	private void Awake()
	{
		_sprite = gameObject.GetComponent<SpriteRenderer>();
		_paddleRB = gameObject.GetComponent<Rigidbody2D>();

		_settings = GameObject.Find("_system").GetComponent<Settings>();
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void ResizePaddle()
	{
		float newSize = 1 + (_sizeMultiplier * 0.25f);
		_sprite.size = new Vector2(newSize, _sprite.size.y);
	}

	private void Move()
	{
		if (_freezePaddle)
		{
			return;
		}

		Vector2 paddleTransform;

		if (Input.GetAxis("Mouse X") != 0)
		{
			paddleTransform = Vector2.right * Input.GetAxis("Mouse X") * _settings.SpeedMouse;
		}
		else
		{
			paddleTransform = Vector2.right * Input.GetAxis("Horizontal") * _settings.SpeedKeyboard;
		}

		_paddleRB.MovePosition(_paddleRB.position + paddleTransform * Time.deltaTime);
	}
}
