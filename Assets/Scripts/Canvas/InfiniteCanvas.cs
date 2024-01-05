using NaughtyAttributes;
using System;
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

	[Header("References")]
	[SerializeField] private RectTransform content;
	[SerializeField] private NodesLibrary nodesLibrary;
	[Header("Settings")]
	[SerializeField] private int framesToWaitWhenGrowing = 25;
	[SerializeField] private float uniformGrowIncrement = 25;
	[Header("Debug")]
	[SerializeField, ReadOnly] private List<Node> nodes = new List<Node>();
	[SerializeField, ReadOnly] private NodeData canvasData;

	public NodeData CanvasData => canvasData;
	public RectTransform Content => content;
	public List<Node> Nodes => nodes;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		TopPanel.OnNavigationEvent += Show;
		CanvasNode.OnCanvasNodeSelected += Show;
		PageCanvas.OnAnyPageCanvasShow += Hide;
		ToolbarNode.OnAnyNodeShouldBeCreated += AddNode;
		Node.OnAnyNodeSelected += DeselectAllNodes;
		Node.OnAnyNodeDragEnd += CheckValidPosition;
		Node.OnAnyNodeDeleted += RemoveNodeFromCanvas;
		NodeData.OnRegeneratedId += SaveNodesList;
	}

	private void OnDestroy()
	{
		TopPanel.OnNavigationEvent -= Show;
		CanvasNode.OnCanvasNodeSelected -= Show;
		PageCanvas.OnAnyPageCanvasShow -= Hide;
		ToolbarNode.OnAnyNodeShouldBeCreated -= AddNode;
		Node.OnAnyNodeSelected -= DeselectAllNodes;
		Node.OnAnyNodeDragEnd -= CheckValidPosition;
		Node.OnAnyNodeDeleted -= RemoveNodeFromCanvas;
		NodeData.OnRegeneratedId -= SaveNodesList;
	}

	private void Start()
	{
		LoadHomeData();
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
	/// Adds a new node on this canvas using position.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
	public void AddNode(Node node, Vector2 position)
	{
		if (IsShown == false) return;
		if (node == null) return;

		Node nodeInstance = Instantiate(node, Vector3.zero, Quaternion.identity, Content);
		Nodes.Add(nodeInstance);

		nodeInstance.RectTransform.position = position;
		CheckValidPosition(nodeInstance);

		nodeInstance.InitializeNode(this);
		SaveNodesList();
	}

	/// <summary>
	/// Adds a new node on this canvas using a loaded node data.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
	public void AddNode(Node node, NodeData nodeData)
	{
		if (IsShown == false) return;
		if (node == null) return;

		Node nodeInstance = Instantiate(node, Vector3.zero, Quaternion.identity, Content);
		Nodes.Add(nodeInstance);

		nodeInstance.InitializeNode(this, nodeData);
		nodeInstance.RectTransform.anchoredPosition = nodeData.AnchoredPosition;

		// HACK: skip a frame to compute the position correctly
		StartCoroutine(Coroutines.DoAfterFrames(1, () =>
		{
			CheckValidPosition(nodeInstance);
			SaveNodesList();
		}));
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
	private void CheckValidPosition(Node nodeInstance)
	{
		if (nodeInstance.WillBeDestroyed) return;

		RectTransform rectTransform = nodeInstance.RectTransform;

		Vector2 overlapDir = Overlap(rectTransform);
		bool isInsideCanvas = overlapDir == Vector2.zero;

		bool isPositionOverlaped = isInsideCanvas == false & TryGrowContentSize(overlapDir, rectTransform) == false;
		nodeInstance.CheckValidPosition(isPositionOverlaped);
	}

	private void CheckIfCanBeSmaller()
	{
		// TODO: reduce canvas size when possible, maybe test if a refresh using base values then growing works
	}

	private void DeselectAllNodes(Node node) => DeselectAllNodes();
	private void Hide(PageCanvas pageCanvas) => Hide();
	private void Show(CanvasNode canvasNode)
	{
		Show();
		LoadCanvasData(canvasNode.NodeData.Id);
	}
	private void Show(string id)
	{
		Show();

		if (id == null)
			LoadHomeData();
		else
			LoadCanvasData(id);
	}

	private void LoadHomeData()
	{
		ClearCanvasContent();
		canvasData = SaveManager.Load<HomeData>(HomeData.IdStatic);

		if (canvasData == null)
		{
			canvasData = new HomeData();
			canvasData.HasInitialized = true;
		}

		if (canvasData.Nodes != null && canvasData.Nodes.Length > 0)
		{
			for (int i = 0; i < canvasData.Nodes.Length; i++)
			{
				string nodeId = canvasData.Nodes[i];
				NodeData nodeData = SaveManager.Load<NodeData>(nodeId);
				AddNode(nodesLibrary.Library[nodeData.Type], nodeData);
			}
		}

		SaveNodesList();
	}

	private void LoadCanvasData(string id)
	{
		ClearCanvasContent();
		canvasData = SaveManager.Load<NodeData>(id);

		if (canvasData == null)
		{
			canvasData = new NodeData();
			canvasData.HasInitialized = true;
		}

		if (canvasData.Nodes != null && canvasData.Nodes.Length > 0)
		{
			for (int i = 0; i < canvasData.Nodes.Length; i++)
			{
				string nodeId = canvasData.Nodes[i];
				NodeData nodeData = SaveManager.Load<NodeData>(nodeId);
				AddNode(nodesLibrary.Library[nodeData.Type], nodeData);
			}
		}

		SaveNodesList();
	}

	private void SaveNodesList()
	{
		if (canvasData == null) return;
		if (Nodes == null) return;

		if (Nodes.Count == 0)
		{
			canvasData.Nodes = new string[0];
			return;
		}

		// using a temporary array because save data will only be saved automatically
		// when actually changing the array values, not its children
		string[] tempNodes = new string[Nodes.Count];
		for (int i = 0; i < Nodes.Count; i++)
			tempNodes[i] = Nodes[i].NodeData.Id;
		canvasData.Nodes = tempNodes;
	}

	private void RemoveNodeFromCanvas(Node node)
	{
		Nodes.Remove(node);
		SaveNodesList();
	}

	private void ClearCanvasContent()
	{
        foreach (Transform child in content.transform)
			Destroy(child.gameObject);

		Nodes.Clear();
    }
	#endregion
}
