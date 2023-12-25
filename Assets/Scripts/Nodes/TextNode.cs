using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextNode : Node
{
	[Header("Debug: TextNode")]
	[SerializeField, ReadOnly] private TMP_InputField textField;

	protected override void Awake()
	{
		base.Awake();
		textField = GetComponent<TMP_InputField>();

		DeselectNode();
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
}
