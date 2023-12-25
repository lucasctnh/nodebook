using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCanvas<T> : Singleton<T> where T : BaseCanvas<T>
{
	[Header("Canvas Settings")]
	[SerializeField] private bool startsHidden;

	protected override bool Awake()
	{
		bool response = base.Awake();

		if (startsHidden)
			Hide();

		return response;
	}

	public static void Hide()
	{
		if (InstanceIsValid == false) return;
		Instance.HideInternal();
	}

	public static void Show()
	{
		if (InstanceIsValid == false) return;
		Instance.ShowInternal();
	}

	protected virtual void HideInternal()
	{
		gameObject.SetActive(false);
	}
	protected virtual void ShowInternal()
	{
		gameObject.SetActive(true);
	}
}
