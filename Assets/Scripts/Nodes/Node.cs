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
	[SerializeField] protected GameObject visualNode;
	[SerializeField] protected GameObject functionalNode;
	[Header("Debug: Node")]
	[SerializeField, ReadOnly] protected bool isSelected;
	[SerializeField, ReadOnly] protected InfiniteCanvas parentCanvas;
	[SerializeField, ReadOnly] protected NodeData nodeData;
	[SerializeField, ReadOnly, ShowIf("useSelfRaycast")] protected Image backgroundImage;

	public abstract NodeType NodeType { get; }
	public bool IsSelected => isSelected;
	public NodeData NodeData => nodeData;
	public GameObject VisualNode => visualNode;
	public GameObject FunctionalNode => functionalNode;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();

		backgroundImage = GetComponent<Image>();
		ActiveFunctionalNode(false);

		if (!useSelfRaycast)
		{
			foreach (var externalGraphic in externalGraphics)
				externalGraphic.OnPointerClicked += OnPointerClick;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!useSelfRaycast)
		{
			foreach (var externalGraphic in externalGraphics)
				externalGraphic.OnPointerClicked -= OnPointerClick;
		}
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
		base.OnEndDrag(eventData);

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
		{
			this.nodeData = nodeData;
			HandleLoadData();
		}
		else
		{
			this.nodeData = new NodeData(NodeType, rectTransform.anchoredPosition, parentCanvas.CanvasData.Id);
			HandleNewData();
		}

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
	protected virtual void ActiveFunctionalNode(bool shouldActive)
	{
		visualNode.SetActive(!shouldActive);
		functionalNode.SetActive(shouldActive);
		if (backgroundImage != null)
			backgroundImage.raycastTarget = shouldActive;
		
		if (!useSelfRaycast)
		{
			foreach (var externalGraphic in externalGraphics)
			{
				externalGraphic.EnableClick(shouldActive);
				Image externBackground = externalGraphic.GetComponent<Image>();
				if (externBackground != null)
					externBackground.raycastTarget = shouldActive;
			}
		}
	}

	protected virtual void HandleLoadData() { }
	protected virtual void HandleNewData() { }
	#endregion
}
