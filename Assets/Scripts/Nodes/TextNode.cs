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
		textField.interactable = false;
	}

	protected override void SelectNode()
	{
		base.SelectNode();
		textField.interactable = true;
	}
}
