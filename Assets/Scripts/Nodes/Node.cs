using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Node : Draggable, IPointerClickHandler//, INodeDeselected
{
	[Header("Debug: Node")]
	[SerializeField, ReadOnly] protected bool isSelected;

	protected bool IsSelected
	{
		get => isSelected;
		set
		{
			isSelected = value;
			isDragActive = !value;
		}
	}

	protected virtual void Update()
	{
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null && IsSelected)
			DeselectNode();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (isDragging) return;
		SelectNode();
	}

	public virtual void SelectNode()
	{
		IsSelected = true;
	}

	public virtual void DeselectNode()
	{
		IsSelected = false;
	}
}
