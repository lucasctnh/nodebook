using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Node : Draggable, IPointerClickHandler//, INodeDeselected
{
	[Header("Debug: Node")]
	[SerializeField, ReadOnly] protected bool isSelected;

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (isDragging) return;
		SelectNode();
	}

	public virtual void SelectNode()
	{
		InfiniteCanvas.DeselectAllNodes();

		isSelected = true;
		isDragActive = false;
	}

	public virtual void DeselectNode()
	{
		isSelected = false;
		isDragActive = true;
	}
}
