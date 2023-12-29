using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public delegate void DragEvent(Draggable draggable);
	public static event DragEvent OnAnyBeginDrag;
	public static event DragEvent OnAnyEndDrag;

	[Header("Settings: Draggable")]
	[SerializeField] protected bool useSelfRaycast = true;
	[SerializeField, HideIf("useSelfRaycast")] protected List<ExternalGraphics> externalGraphics = new List<ExternalGraphics>();
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

		SubscribeToExternalEvents();
	}

	protected virtual void OnDestroy()
	{
		UnsubscribeToExternalEvents();
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (isDragActive == false) return;

		anchoredPositionBeforeDrag = rectTransform.anchoredPosition;
		isDragging = true;

		OnAnyBeginDrag?.Invoke(this);
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		if (isDragActive == false) return;
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (isDragActive == false) return;

		isDragging = false;
		OnAnyEndDrag?.Invoke(this);
	}

	protected virtual void SubscribeToExternalEvents()
	{
		if (!useSelfRaycast)
		{
			foreach (var externalGraphic in externalGraphics)
			{
				externalGraphic.OnBeginedDrag += OnBeginDrag;
				externalGraphic.OnEndedDrag += OnEndDrag;
				externalGraphic.OnDragging += OnDrag;
			}
		}
	}

	protected virtual void UnsubscribeToExternalEvents()
	{
		if (!useSelfRaycast)
		{
			foreach (var externalGraphic in externalGraphics)
			{
				externalGraphic.OnBeginedDrag -= OnBeginDrag;
				externalGraphic.OnEndedDrag -= OnEndDrag;
				externalGraphic.OnDragging -= OnDrag;
			}
		}
	}
}
