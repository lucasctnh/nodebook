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

		DeselectNode();
	}

	// HACK: on inspector Im manually selecting the input field because theres some weird behaviour
	// the select node is not called on clicking on input field directly, but also is not calling the event system select
	public override void SelectNode()
	{
		if (isSelected)
		{
			ViewManager.ShowPageCanvas();
			return;
		}

		base.SelectNode();
		textField.interactable = true;

		if (textField.text == string.Empty || textField.text == "New Page")
			textField.Select();
	}

	public override void DeselectNode()
	{
		base.DeselectNode();
		textField.interactable = false;
	}
}
