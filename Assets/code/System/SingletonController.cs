using UnityEngine;

public class SingletonController : MonoBehaviour
{
	public static GameObject Singleton;

	private void Awake()
	{
		if (Singleton != null && Singleton != gameObject)
		{
			Destroy(gameObject);
			return;
		}

		Singleton = gameObject;
	}
}
