using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables
	[SerializeField] private int _pointValue = 1;
	[SerializeField] private int _maxHealth = 1;
	[SerializeField] private Sprite[] _sprites = null;
	[SerializeField] private float _pickupSpawnChance = 0.1f;
	[SerializeField] private Transform[] _pickups;

	private int _health;

	// References
	private SpriteRenderer _spriteRenderer;
	private Gameplay _game;
	private GameScore _gameScore;
	#endregion

	public void Damage(int power)
	{
		_health -= power;
		AdjustSprite();

		if (_health <= 0)
		{
			SpawnPickup();

			if (gameObject.tag == "barrier")
			{
				_gameScore.ChangeScore(_pointValue);
			}
			else
			{
				_game.ChangeProgress(_pointValue);
			}

			gameObject.SetActive(false);
		}
	}

	public void ResetBlock()
	{
		gameObject.SetActive(true);
		_health = _maxHealth;
		AdjustSprite();
	}

	private void Start()
	{
		_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		_game = GameObject.Find("_system").GetComponent<Gameplay>();
		_gameScore = GameObject.Find("_system").GetComponent<GameScore>();
		ResetBlock();
	}

	private void SpawnPickup()
	{
		if (_pickups.Length < 1)
		{
			return;
		}

		float dropRandomization = Random.Range(0f, 1f);

		if (dropRandomization <= _pickupSpawnChance)
		{
			int pickupType = Random.Range(0, _pickups.Length);
			Instantiate(_pickups[pickupType], transform.position, Quaternion.identity);
		}
	}

	private void AdjustSprite()
	{
		if (_spriteRenderer != null && _sprites.Length > 0)
		{
			int spriteIndex = (_health <= 0) ? 0 : _maxHealth - _health;
			_spriteRenderer.sprite = _sprites[spriteIndex];
		}
	}
}
