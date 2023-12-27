using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	[Header("Singleton")]
	[SerializeField] private bool dontDestroyOnLoad = true;

	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance != null)
				return instance;

			try
			{
				instance = (T)FindObjectOfType(typeof(T));
			}
			catch (System.Exception e)
			{
				//Debug.LogError($"Error grabbing Singleton instance: {e}");
				return null;
			}

			return instance;
		}
	}

	public static bool IsInstanceValid => Instance != null;

	protected virtual bool Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return false;
		}

		if (dontDestroyOnLoad)
			DontDestroyOnLoad(gameObject);

		return true;
	}
}
