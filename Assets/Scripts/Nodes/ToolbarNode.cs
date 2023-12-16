using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolbarNode : Draggable
{
	[Header("References")]
	[SerializeField] private Image toolbarVisual;
	[SerializeField] private Node nodePrefab;
	[Header("Debug: ToolbarNode")]
	[SerializeField, ReadOnly] private Node nodeInstance;
	[SerializeField, ReadOnly] private Vector3 initialPosition;

	protected override void Awake()
	{
		base.Awake();
		initialPosition = rectTransform.anchoredPosition;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);

		toolbarVisual.enabled = false;
		nodeInstance = Instantiate(nodePrefab, rectTransform.position, Quaternion.identity, rectTransform);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);

		toolbarVisual.enabled = true;

		// TODO: check if inside canvas before
		InfiniteCanvas.AddNode(nodeInstance, rectTransform.position);

		rectTransform.anchoredPosition = initialPosition;
		Destroy(nodeInstance.gameObject);
	}
}
