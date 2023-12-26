using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
	public delegate void TopPanelEvent();
	public static event TopPanelEvent OnShowInfiniteCanvasEvent;

	[Header("References")]
	[SerializeField] private Button homeButton;

	private void Awake()
	{
		Assert.IsNotNull(homeButton);
		homeButton.onClick.AddListener(RaiseShowInifiniteCanvasEvent);
	}

	private void OnDestroy()
	{
		homeButton.onClick.RemoveListener(RaiseShowInifiniteCanvasEvent);
	}

	private void RaiseShowInifiniteCanvasEvent() => OnShowInfiniteCanvasEvent?.Invoke();
}
