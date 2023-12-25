using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageNode : Node
{
	[Header("Debug: PageNode")]
	[SerializeField, ReadOnly] private TMP_InputField textField;

	protected override void Awake()
	{
		base.Awake();
		textField = GetComponentInChildren<TMP_InputField>();
		textField.interactable = false;
	}

	protected override void SelectNode()
	{
		base.SelectNode();
		textField.interactable = true;

		if (textField.text == string.Empty || textField.text == "New Page")
			textField.Select();
	}
}
