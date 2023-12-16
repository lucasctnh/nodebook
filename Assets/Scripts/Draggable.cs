using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[Header("Debug: Draggable")]
	[SerializeField, ReadOnly] protected Canvas canvas;
	[SerializeField, ReadOnly] protected RectTransform rectTransform;
	[SerializeField, ReadOnly] protected Vector2 anchoredPositionBeforeDrag;
	[SerializeField, ReadOnly] protected bool isDragActive;
	[SerializeField, ReadOnly] protected bool isDragging;

	public RectTransform RectTransform => rectTransform;

	protected virtual void Awake()
	{
		isDragActive = true;
		canvas = GetComponentInParent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (isDragActive == false) return;

		anchoredPositionBeforeDrag = rectTransform.anchoredPosition;
		isDragging = true;
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		if (isDragActive == false) return;
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (isDragActive == false) return;

		CheckValidPosition();
		isDragging = false;
	}

	public void CheckValidPosition()
	{
		Vector2 overlapDir = InfiniteCanvas.Overlap(rectTransform);
		bool isInsideCanvas = overlapDir == Vector2.zero;

		if (isInsideCanvas == false & InfiniteCanvas.TryGrowContentSize(overlapDir, rectTransform) == false)
			rectTransform.anchoredPosition = anchoredPositionBeforeDrag;
	}
}
