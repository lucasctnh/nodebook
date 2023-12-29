using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextNode : Node
{
	[Header("Debug: TextNode")]
	[SerializeField, ReadOnly] private TMP_InputField textField;

	public override NodeType NodeType => NodeType.Text;

	protected override void Awake()
	{
		base.Awake();
		textField = GetComponentInChildren<TMP_InputField>(true);
		textField.onValueChanged.AddListener(SaveNodeStringData);

		DeselectNode();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		textField.onValueChanged.RemoveListener(SaveNodeStringData);
	}

	public override void SelectNode()
	{
		base.SelectNode();
		textField.interactable = true;

		if (textField.text == string.Empty)
			textField.Select();
	}

	public override void DeselectNode()
	{
		base.DeselectNode();
		textField.interactable = false;
	}

	protected override void HandleLoadData()
	{
		textField.text = nodeData.Content;
	}

	private void SaveNodeStringData(string stringData)
	{
		if (nodeData != null && nodeData.HasInitialized)
		{
			nodeData.Content = stringData;
		}
	}
}
