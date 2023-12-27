using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Node : Draggable, IPointerClickHandler
{
	public delegate void NodeEvent(Node node);
	public static event NodeEvent OnAnyNodeSelected;
	public static event NodeEvent OnAnyNodeDragEnd;

	[Header("References: Node")]
	[SerializeField] private GameObject visualNode;
	[SerializeField] private GameObject functionalNode;
	[Header("Debug: Node")]
	[SerializeField, ReadOnly] protected bool isSelected;
	[SerializeField, ReadOnly] protected InfiniteCanvas parentCanvas;
	[SerializeField, ReadOnly] protected NodeData nodeData;
	[SerializeField, ReadOnly, ShowIf("useSelfRaycast")] protected Image backgroundImage;
	[SerializeField, ReadOnly, HideIf("useSelfRaycast")] protected Image externBackgroundImage;

	public abstract NodeType NodeType { get; }
	public NodeData NodeData => nodeData;
	protected Image BackgroundImage
	{
		get
		{
			if (useSelfRaycast)
			{
				if (backgroundImage == null)
					backgroundImage = GetComponent<Image>();
				return backgroundImage;
			}
			else
			{
				if (externBackgroundImage == null)
					externBackgroundImage = externalGraphics.GetComponent<Image>();
				return externBackgroundImage;
			}
		}
	}

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		ActiveFunctionalNode(false);

		if (!useSelfRaycast)
			externalGraphics.OnPointerClicked += OnPointerClick;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!useSelfRaycast)
			externalGraphics.OnPointerClicked -= OnPointerClick;
	}
	#endregion

	#region Interface Implementation
	public void OnPointerClick(PointerEventData eventData)
	{
		if (isDragging) return;
		SelectNode();
	}
	#endregion

	#region Override Methods
	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);

		if (nodeData != null)
		{
			nodeData.AnchoredPosition = rectTransform.anchoredPosition;
			nodeData.RegenerateId();
		}

		OnAnyNodeDragEnd?.Invoke(this);
	}
	#endregion

	#region Public Methods
	public virtual void InitializeNode(InfiniteCanvas canvas, NodeData nodeData = null)
	{
		parentCanvas = canvas;
		if (nodeData != null)
			this.nodeData = nodeData;
		else
			this.nodeData = new NodeData(NodeType, rectTransform.anchoredPosition, parentCanvas.CanvasData.Id);

		ActiveFunctionalNode(true);
	}

	public virtual void DestroyNode()
	{
		DeselectNode();
		Destroy(gameObject);
	}

	public virtual void SelectNode()
	{
		OnAnyNodeSelected?.Invoke(this);

		isSelected = true;
		isDragActive = false;
	}

	public virtual void DeselectNode()
	{
		isSelected = false;
		isDragActive = true;
	}

	public void CheckValidPosition(bool hasOverlaped)
	{
		if (hasOverlaped)
		{
			rectTransform.anchoredPosition = anchoredPositionBeforeDrag;

			if (nodeData != null)
			{
				nodeData.AnchoredPosition = rectTransform.anchoredPosition;
				nodeData.RegenerateId();
			}
		}
	}
	#endregion

	#region Protected Methods
	protected void ActiveFunctionalNode(bool shouldActive)
	{
		visualNode.SetActive(!shouldActive);
		functionalNode.SetActive(shouldActive);
		BackgroundImage.raycastTarget = shouldActive;
		
		if (!useSelfRaycast)
			externalGraphics.EnableClick(shouldActive);
	}
	#endregion
}
