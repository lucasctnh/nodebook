using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InfiniteCanvas : Singleton<InfiniteCanvas>
{
	[Header("Settingfs")]
	[SerializeField] private int framesToWaitWhenGrowing = 25;
	[SerializeField] private float uniformGrowIncrement = 25;
	[Header("References")]
	[SerializeField] private RectTransform content;
	[Header("Debug")]
	[SerializeField, ReadOnly] private List<Node> nodes = new List<Node>();

	public static RectTransform Content => InstanceIsValid ? Instance.content : null;
	public static List<Node> Nodes => InstanceIsValid ? Instance.nodes : new List<Node>();

	public static Vector2 Overlap(RectTransform rectTransform)
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

	public static bool TryGrowContentSize(Vector2 growDirection, RectTransform rectOutsideCanvas)
	{
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

	public static void AddNode(Node node, Vector2 position)
	{
		if (node == null) return;

		Node nodeInstance = Instantiate(node, Vector3.zero, Quaternion.identity, Content);
		Nodes.Add(nodeInstance);

		nodeInstance.RectTransform.position = position;
		nodeInstance.CheckValidPosition();
	}

	private void CheckIfCanBeSmaller()
	{

	}
}
