using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCanvas : MonoBehaviour
{
	[Header("Canvas Settings")]
	[SerializeField] private bool startsHidden;
	[Header("Debug: Canvas")]
	[SerializeField, ReadOnly] private bool isShown;

	public bool IsShown => isShown;

	protected virtual void Awake()
	{
		isShown = true;

		if (startsHidden)
			Hide();
	}

	public virtual void Show()
	{
		gameObject.SetActive(true);
		isShown = true;
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
		isShown = false;
	}
}
