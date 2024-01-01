using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Debug")]
	[SerializeField, ReadOnly] private Node currentDragging;
	[SerializeField, ReadOnly] private bool isPointerOnTrash;

	private void Awake()
	{
		Node.OnAnyNodeDragBegin += CacheCurrentDrag;
		Node.OnAnyNodeDragEndBeforeSave += TryDestroyNode;
	}

	private void OnDestroy()
	{
		Node.OnAnyNodeDragBegin -= CacheCurrentDrag;
		Node.OnAnyNodeDragEndBeforeSave -= TryDestroyNode;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isPointerOnTrash = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isPointerOnTrash = false;
	}

	private void CacheCurrentDrag(Node node) => currentDragging = node;
	private void TryDestroyNode(Node node)
	{
		if (isPointerOnTrash == false) return;
		if (currentDragging == false) return;

		node.WillBeDestroyed = true;
		node.DestroyNode();
	}
}
