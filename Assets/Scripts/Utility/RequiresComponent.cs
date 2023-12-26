using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RequiresComponent<T> : MonoBehaviour where T : MonoBehaviour
{
	private T requiredComponent;
	protected T RequiredComponent => requiredComponent == null ? requiredComponent = GetComponent<T>() : requiredComponent;

	protected virtual void OnValidate()
	{
		if (RequiredComponent == null)
			gameObject.AddComponent<T>();
	}
}
