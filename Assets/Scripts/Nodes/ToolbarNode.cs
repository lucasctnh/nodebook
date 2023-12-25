using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolbarNode : Draggable
{
	[Header("References")]
	[SerializeField] private List<Image> toolbarVisuals = new List<Image>();
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

		HideVisuals();
		nodeInstance = Instantiate(nodePrefab, rectTransform.position, Quaternion.identity, rectTransform);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);

		ShowVisuals();

		// TODO: check if left toolbar space before creating node
		InfiniteCanvas.AddNode(nodeInstance, rectTransform.position);

		rectTransform.anchoredPosition = initialPosition;
		Destroy(nodeInstance.gameObject);
	}

	private void HideVisuals()
	{
		foreach (var visual in toolbarVisuals)
			visual.enabled = false;
	}

	private void ShowVisuals()
	{
		foreach (var visual in toolbarVisuals)
			visual.enabled = true;
	}
}
