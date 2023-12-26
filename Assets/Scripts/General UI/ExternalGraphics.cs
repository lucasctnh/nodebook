using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExternalGraphics : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public delegate void PointerEvent(PointerEventData eventData);
	public delegate void DragEvent(PointerEventData eventData);

	public event PointerEvent OnPointerEntered;
	public event PointerEvent OnPointerExited;
	public event PointerEvent OnPointerClicked;

	public event DragEvent OnBeginedDrag;
	public event DragEvent OnEndedDrag;
	public event DragEvent OnDragging;

	[Header("Debug")]
	[SerializeField, ReadOnly] private Image clickableImg;

	#region Interface Implementation
	private void Awake()
	{
		clickableImg = GetComponent<Image>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnPointerClicked?.Invoke(eventData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnPointerEntered?.Invoke(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnPointerExited?.Invoke(eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		OnBeginedDrag?.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		OnEndedDrag?.Invoke(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnDragging?.Invoke(eventData);
	}
	#endregion

	public void EnableClick(bool isEnabled)
	{
		if (clickableImg == null)
			clickableImg = GetComponent<Image>();

		clickableImg.raycastTarget = isEnabled;
	}
}
