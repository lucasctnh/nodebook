using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler
{
	public RectTransform content;
	private Canvas canvas;
	private RectTransform rectTransform;

	private void Awake()
	{
		canvas = GetComponentInParent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
	}
	private void Update()
	{
		if (IsRectTransformInsideContent(content, rectTransform))
		{
			Debug.Log("Element is contained in Content");
		}
		else
		{
			Debug.Log("Element is not contained in Content");
			GrowContentSize();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	private bool IsRectTransformInsideContent(RectTransform content, RectTransform rectTransform)
	{
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);

		foreach (Vector3 corner in corners)
		{
			// I am not sure (its not documented) but I think that if dont pass
			// the Camera param Unity will convert into world space
			// so, if you pass the param you need to first convert the points into Screen space
			// Vector3 screenCorner = Camera.main.WorldToScreenPoint(corner);

			if (!RectTransformUtility.RectangleContainsScreenPoint(content, corner))
				return false;
		}

		return true;
	}

	private void GrowContentSize()
	{
		// just a test
		content.sizeDelta += new Vector2(50, 50);
	}
}
