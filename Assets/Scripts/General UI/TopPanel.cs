using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
	public delegate void TopPanelEvent(string canvasId);
	public static event TopPanelEvent OnShowHomeEvent;

	[Header("References")]
	[SerializeField] private Button homeButton;

	private void Awake()
	{
		Assert.IsNotNull(homeButton);
		homeButton.onClick.AddListener(RaiseShowHomeEvent);
	}

	private void OnDestroy()
	{
		homeButton.onClick.RemoveListener(RaiseShowHomeEvent);
	}

	private void RaiseShowHomeEvent() => OnShowHomeEvent?.Invoke(null);
}
