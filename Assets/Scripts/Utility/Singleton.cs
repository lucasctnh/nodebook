using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	[Header("Singleton")]
	[SerializeField] private bool dontDestroyOnLoad = true;

	#region Properties
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = (T)FindObjectOfType(typeof(T));

			return instance;
		}
	}
	public static bool InstanceIsValid => Instance != null;
	#endregion

	#region Unity Messages
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
	#endregion
}
