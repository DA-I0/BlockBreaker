using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonControler : MonoBehaviour
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
