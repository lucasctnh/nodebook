using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageCanvas : BaseCanvas
{
	public delegate void PageCanvasEvent(PageCanvas pageCanvas);
	public static event PageCanvasEvent OnAnyPageCanvasShow;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		InfiniteCanvas.OnAnyInfiniteCanvasShow += HideCanvas;
		PageNode.OnPageNodeSelected += ShowCanvas;
	}

	private void OnDestroy()
	{
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
	private void ShowCanvas(PageNode node) => Show();
	private void HideCanvas(InfiniteCanvas canvas) => Hide();
	#endregion
}
