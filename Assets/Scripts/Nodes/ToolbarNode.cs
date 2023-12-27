using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolbarNode : Draggable
{
	public delegate void ToolbarNodeEvent(ToolbarNode node);
	public delegate void ToolbarAddNodeEvent(Node node, Vector2 position);
	public static event ToolbarAddNodeEvent OnAnyNodeShouldBeCreated;

	[Header("References")]
	[SerializeField] private List<Image> toolbarVisuals = new List<Image>();
	[SerializeField] private NodeType nodeType;
	[SerializeField] private NodesLibrary nodesLibrary;
	[Header("Debug: ToolbarNode")]
	[SerializeField, ReadOnly] private Node nodeInstance;
	[SerializeField, ReadOnly] private Vector3 initialPosition;
	[SerializeField, ReadOnly] private bool isPointerOnCanvas;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		InfiniteCanvas.OnPointerEnterCanvas += OnCanvasPointerEnter;
		InfiniteCanvas.OnPointerExitCanvas += OnCanvasPointerExit;

		initialPosition = rectTransform.anchoredPosition;
	}

	private void OnDestroy()
	{
		InfiniteCanvas.OnPointerEnterCanvas -= OnCanvasPointerEnter;
		InfiniteCanvas.OnPointerExitCanvas -= OnCanvasPointerExit;
	}
	#endregion

	#region Override Methods
	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		DisableRaycast();
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);

		if (isPointerOnCanvas)
		{
			HideVisuals();
			if (nodeInstance == null)
				nodeInstance = Instantiate(nodesLibrary.Library[nodeType], rectTransform.position, Quaternion.identity, rectTransform);
		}
		else
		{
			ShowVisuals();
			if (nodeInstance != null)
				Destroy(nodeInstance.gameObject);
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);

		// HACK: for some reason needs to hide and show for UI to update position correctly
		HideVisuals();
		EnableRaycast();
		ShowVisuals();

		if (isPointerOnCanvas)
			OnAnyNodeShouldBeCreated?.Invoke(nodeInstance, rectTransform.position);

		rectTransform.anchoredPosition = initialPosition;

		if (nodeInstance != null)
			Destroy(nodeInstance.gameObject);
	}
	#endregion

	#region Private Methods
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

	private void DisableRaycast()
	{
		foreach (var visual in toolbarVisuals)
			visual.raycastTarget = false;
	}

	private void EnableRaycast()
	{
		foreach (var visual in toolbarVisuals)
			visual.raycastTarget = true;
	}

	private void OnCanvasPointerEnter(InfiniteCanvas canvas) => isPointerOnCanvas = true;
	private void OnCanvasPointerExit(InfiniteCanvas canvas) => isPointerOnCanvas = false;
	#endregion
}
