using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageCanvas : BaseCanvas
{
	public delegate void PageCanvasEvent(PageCanvas pageCanvas);
	public static event PageCanvasEvent OnAnyPageCanvasShow;

	[Header("Debug")]
	[SerializeField, ReadOnly] private NodeData pageData;
	[SerializeField, ReadOnly] private TMP_InputField inputField;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		inputField = GetComponentInChildren<TMP_InputField>(true);
		inputField.onValueChanged.AddListener(SavePageData);

		InfiniteCanvas.OnAnyInfiniteCanvasShow += HideCanvas;
		PageNode.OnPageNodeSelected += ShowCanvas;
	}

	private void OnDestroy()
	{
		inputField.onValueChanged.RemoveListener(SavePageData);

		InfiniteCanvas.OnAnyInfiniteCanvasShow -= HideCanvas;
		PageNode.OnPageNodeSelected -= ShowCanvas;
	}
	#endregion

	#region Override Methods
	public override void Show()
	{
		base.Show();
		OnAnyPageCanvasShow?.Invoke(this);
	}
	#endregion

	#region Private Methods
	private void ShowCanvas(PageNode node)
	{
		pageData = node.NodeData;
		if (pageData.Content != null && pageData.Content[0] != string.Empty)
			inputField.text = pageData.Content[0];

		Show();
	}

	private void HideCanvas(InfiniteCanvas canvas) => Hide();

	private void SavePageData(string data)
	{
		if (pageData.Content == null)
			pageData.Content = new string[1];

		pageData.Content[0] = data;
	}
	#endregion
}
