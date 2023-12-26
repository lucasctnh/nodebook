using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfiniteCanvas : BaseCanvas, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public delegate void InfiniteCanvasCheckOverlapEvent(bool hasOverlaped);
	public delegate void InfiniteCanvasEvent(InfiniteCanvas infiniteCanvas);
	public static event InfiniteCanvasEvent OnAnyInfiniteCanvasShow;
	public static event InfiniteCanvasEvent OnPointerEnterCanvas;
	public static event InfiniteCanvasEvent OnPointerExitCanvas;
	public static event InfiniteCanvasCheckOverlapEvent OnPositionCheckReturned;

	[Header("References")]
	[SerializeField] private RectTransform content;

	[Header("Settings")]
	[SerializeField] private int framesToWaitWhenGrowing = 25;
	[Header("Debug")]
	[SerializeField, ReadOnly] private List<Node> nodes = new List<Node>();
	[SerializeField] private float uniformGrowIncrement = 25;

	public RectTransform Content => content;
	public List<Node> Nodes => nodes;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		TopPanel.OnShowInfiniteCanvasEvent += Show;
		PageCanvas.OnAnyPageCanvasShow += Hide;
		ToolbarNode.OnAnyNodeShouldBeCreated += AddNode;
		Node.OnAnyNodeSelected += DeselectAllNodes;
		Node.OnAnyNodeRequestOverlap += CheckDraggablePosition;
	}

	private void OnDestroy()
	{
		TopPanel.OnShowInfiniteCanvasEvent -= Show;
		PageCanvas.OnAnyPageCanvasShow -= Hide;
		ToolbarNode.OnAnyNodeShouldBeCreated -= AddNode;
		Node.OnAnyNodeSelected -= DeselectAllNodes;
		Node.OnAnyNodeRequestOverlap -= CheckDraggablePosition;
	}
	#endregion

	#region Static Methods
	/// <summary>
	/// Checks if a rectTransform is overlapping with the viewbox area of the canvas. Used to grow the viewport size when needed.
	/// </summary>
	/// <param name="rectTransform"></param>
	/// <returns></returns>
	public Vector2 Overlap(RectTransform rectTransform)
	{
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);

		bool[] cornersOutside = new bool[4];
		for (int i = 0; i < corners.Length; i++)
		{
			// I am not sure (its not documented) but I think that if dont pass
			// the Camera param Unity will convert into world space
			// so, if you pass the param you need to first convert the points into Screen space
			// Vector3 screenCorner = Camera.main.WorldToScreenPoint(corner);

			cornersOutside[i] = false;
			if (!RectTransformUtility.RectangleContainsScreenPoint(content, corners[i]))
			{
				// corner 0 is left-bot, 1 is left-top, 2 is right-top, 3 is right-bot
				cornersOutside[i] = true;
			}
		}

		Vector2 resultDir = Vector2.zero;
		if (cornersOutside[2] && cornersOutside[3])
			resultDir.x = 1; // overlapping on right
		if (cornersOutside[0] && cornersOutside[3])
			resultDir.y = 1; // overlapping on bottom

		if (cornersOutside[0] && cornersOutside[1])
			resultDir.x = -1; // overlapping on left
		if (cornersOutside[1] && cornersOutside[2])
			resultDir.y = -1; // overlapping on top

		return resultDir;
	}

	/// <summary>
	/// Tries to grow the viewbox size to fit all the nodes.
	/// </summary>
	/// <param name="growDirection"></param>
	/// <param name="rectOutsideCanvas"></param>
	/// <returns></returns>
	public bool TryGrowContentSize(Vector2 growDirection, RectTransform rectOutsideCanvas)
	{
		float growWidth = 0;
		float growHeight = 0;

		if (growDirection.x == 1)
			growWidth = rectOutsideCanvas.sizeDelta.x + uniformGrowIncrement;
		if (growDirection.y == 1)
			growHeight = rectOutsideCanvas.sizeDelta.y + uniformGrowIncrement;

		// TODO: fix case where object is invalid on one dir
		if (growWidth == 0 && growHeight == 0)
			return false;

		Content.sizeDelta += new Vector2(growWidth, growHeight);
		return true;
	}

	/// <summary>
	/// Adds a new node on this canvas.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
	public void AddNode(Node node, Vector2 position)
	{
		if (IsShown == false) return;
		if (node == null) return;

		Node nodeInstance = Instantiate(node, Vector3.zero, Quaternion.identity, Content);
		nodeInstance.InitializeNode(this);
		Nodes.Add(nodeInstance);

		nodeInstance.RectTransform.position = position;
		nodeInstance.RequestValidPositionCheck();
	}

	/// <summary>
	/// Deselects all nodes in this canvas list.
	/// </summary>
	public void DeselectAllNodes()
	{
		foreach (var node in Nodes)
			node.DeselectNode();
	}
	#endregion

	#region Override Methods
	public override void Show()
	{
		base.Show();
		OnAnyInfiniteCanvasShow?.Invoke(this);
	}

	public override void Hide()
	{
		DeselectAllNodes();
		base.Hide();
	}
	#endregion

	#region Interface Implementation
	public void OnPointerClick(PointerEventData eventData)
	{
		DeselectAllNodes();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnPointerEnterCanvas?.Invoke(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnPointerExitCanvas?.Invoke(this);
	}
	#endregion

	#region Private Methods
	private void CheckDraggablePosition(Draggable draggable)
	{
		RectTransform rectTransform = draggable.RectTransform;

		Vector2 overlapDir = Overlap(rectTransform);
		bool isInsideCanvas = overlapDir == Vector2.zero;

		bool isPositionOverlaped = isInsideCanvas == false & TryGrowContentSize(overlapDir, rectTransform) == false;
		OnPositionCheckReturned?.Invoke(isPositionOverlaped);
	}

	private void CheckIfCanBeSmaller()
	{
		// TODO: reduce canvas size when possible, maybe test if a refresh using base values then growing works
	}

	private void DeselectAllNodes(Node node) => DeselectAllNodes();
	private void Hide(PageCanvas pageCanvas) => Hide();
	#endregion
}
