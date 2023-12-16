using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[Header("Debug")]
	[SerializeField, ReadOnly] private Canvas canvas;
	[SerializeField, ReadOnly] private RectTransform rectTransform;
	[SerializeField, ReadOnly] private Vector2 anchoredPositionBeforeDrag;

	private void Awake()
	{
		canvas = GetComponentInParent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		anchoredPositionBeforeDrag = rectTransform.anchoredPosition;
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Vector2 overlapDir = InfiniteCanvas.Overlap(rectTransform);
		bool isInsideCanvas = overlapDir == Vector2.zero;

		if (isInsideCanvas == false & InfiniteCanvas.TryGrowContentSize(overlapDir, rectTransform) == false)
			rectTransform.anchoredPosition = anchoredPositionBeforeDrag;
	}
}
