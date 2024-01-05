using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
	public delegate void NavigationEvent(NavigationButton navButton);
	public static event NavigationEvent OnAnyNavigation;

	[Header("References")]
	[SerializeField] private TMP_Text textName;
	[SerializeField] private Button button;
	[Header("Debug")]
	[SerializeField, ReadOnly] private NodeData nodeData;
	[SerializeField, ReadOnly] private bool isLast;

	public string Name
	{
		get => textName.text;
		set => textName.text = value;
	}

	public NodeData NodeData
	{
		get => nodeData;
		set => nodeData = value;
	}

	public bool IsLast
	{
		get => isLast;
		set => isLast = value;
	}

	private void Awake()
	{
		button.onClick.AddListener(RaiseNavigationEvent);
	}

	private void OnDestroy()
	{
		button.onClick.RemoveListener(RaiseNavigationEvent);
	}

	private void RaiseNavigationEvent()
	{
		if (IsLast) return;
		OnAnyNavigation?.Invoke(this);
	}
}
