using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Debug")]
	[SerializeField, ReadOnly] private Draggable currentDragging;
	[SerializeField, ReadOnly] private bool isPointerOnTrash;

	private void Awake()
	{
		Draggable.OnAnyBeginDrag += CacheCurrentDrag;
		Draggable.OnAnyEndDrag += TryDestroyNode;
	}

	private void OnDestroy()
	{
		Draggable.OnAnyBeginDrag -= CacheCurrentDrag;
		Draggable.OnAnyEndDrag -= TryDestroyNode;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isPointerOnTrash = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isPointerOnTrash = false;
	}

	private void CacheCurrentDrag(Draggable draggable) => currentDragging = draggable;
	private void TryDestroyNode(Draggable draggable)
	{
		if (isPointerOnTrash == false) return;
		if (currentDragging == false) return;

		Node node = draggable as Node;
		if (node == null)
		{
			Debug.LogError("Could not cast draggable as Node");
			return;
		}

		node.DestroyNode();
	}
}
