using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CanvasNode : Node
{
	public delegate void CanvasNodeEvent(CanvasNode node);
	public static event CanvasNodeEvent OnCanvasNodeSelected;

	[Header("Settings: CanvasNode")]
	[SerializeField] private string defaultName = "New Canvas";
	[Header("Debug: CanvasNode")]
	[SerializeField, ReadOnly] private TMP_InputField textField;

	public override NodeType NodeType => NodeType.Canvas;

	protected override void Awake()
	{
		base.Awake();
		textField = GetComponentInChildren<TMP_InputField>(true);
		textField.text = defaultName;

		textField.onValueChanged.AddListener(OnValueChanged);

		DeselectNode();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		textField.onValueChanged.RemoveListener(OnValueChanged);
	}

	// HACK: on inspector Im manually selecting the input field because theres some weird behaviour
	// the select node is not called on clicking on input field directly, but also is not calling the event system select
	public override void SelectNode()
	{
		if (isSelected)
		{
			OnCanvasNodeSelected?.Invoke(this);
			return;
		}

		base.SelectNode();
		textField.interactable = true;

		if (textField.text == string.Empty || textField.text == defaultName)
			textField.Select();
	}

	public override void DeselectNode()
	{
		base.DeselectNode();
		textField.interactable = false;
	}

	protected override void HandleNewData()
	{
		nodeData.Name = defaultName;
	}

	protected override void HandleLoadData()
	{
		textField.text = nodeData.Name;
	}

	private void OnValueChanged(string stringData)
	{
		if (nodeData != null && nodeData.HasInitialized)
		{
			nodeData.Name = stringData;
		}
	}
}
