using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfiniteCanvas : BaseCanvas<InfiniteCanvas>, IPointerClickHandler
{
	[Header("Settings")]
	[SerializeField] private int framesToWaitWhenGrowing = 25;
	[SerializeField] private float uniformGrowIncrement = 25;
	[Header("References")]
	[SerializeField] private RectTransform content;
	[Header("Debug")]
	[SerializeField, ReadOnly] private List<Node> nodes = new List<Node>();

	public static RectTransform Content => InstanceIsValid ? Instance.content : null;
	public static List<Node> Nodes => InstanceIsValid ? Instance.nodes : new List<Node>();

	#region Static Methods
	/// <summary>
	/// Checks if a rectTransform is overlapping with the viewbox area of the canvas. Used to grow the viewport size when needed.
	/// </summary>
	/// <param name="rectTransform"></param>
	/// <returns></returns>
	public static Vector2 Overlap(RectTransform rectTransform)
	{
		if (InstanceIsValid == false) return Vector2.zero;

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
			if (!RectTransformUtility.RectangleContainsScreenPoint(Instance.content, corners[i]))
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
	public static bool TryGrowContentSize(Vector2 growDirection, RectTransform rectOutsideCanvas)
	{
		if (InstanceIsValid == false) return false;

		float growWidth = 0;
		float growHeight = 0;

		if (growDirection.x == 1)
			growWidth = rectOutsideCanvas.sizeDelta.x + Instance.uniformGrowIncrement;
		if (growDirection.y == 1)
			growHeight = rectOutsideCanvas.sizeDelta.y + Instance.uniformGrowIncrement;

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
	public static void AddNode(Node node, Vector2 position)
	{
		if (InstanceIsValid == false) return;
		if (node == null) return;

		Node nodeInstance = Instantiate(node, Vector3.zero, Quaternion.identity, Content);
		Nodes.Add(nodeInstance);

		nodeInstance.RectTransform.position = position;
		nodeInstance.CheckValidPosition();
	}

	/// <summary>
	/// Deselects all nodes in this canvas list.
	/// </summary>
	public static void DeselectAllNodes()
	{
		if (InstanceIsValid == false) return;

		foreach (var node in Nodes)
			node.DeselectNode();
	}
	#endregion

	#region Interface Implementation
	public void OnPointerClick(PointerEventData eventData)
	{
		DeselectAllNodes();
	}
	#endregion

	#region Override Methods
	protected override void HideInternal()
	{
		DeselectAllNodes();
		base.HideInternal();
	}
	#endregion

	#region Private Methods
	private void CheckIfCanBeSmaller()
	{
		// TODO: reduce canvas size when possible, maybe test if a refresh using base values then growing works
	}
	#endregion
}
