using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TopPanel : BaseCanvas<TopPanel>
{
	[Header("References")]
	[SerializeField] private Button homeButton;

	protected override bool Awake()
	{
		bool response = base.Awake();

		Assert.IsNotNull(homeButton);
		homeButton.onClick.AddListener(ViewManager.ShowInfiniteCanvas);

		return response;
	}

	private void OnDestroy()
	{
		homeButton.onClick.RemoveListener(ViewManager.ShowInfiniteCanvas);
	}
}
