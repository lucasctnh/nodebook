using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextNodeInputField : RequiresParent<TextNode>
{
	[SerializeField, ReadOnly] private TMP_InputField inputField;
	[SerializeField, ReadOnly] private int caretPos;
	[SerializeField, ReadOnly] private int lengthForCaret;
	[SerializeField, ReadOnly] private bool hasAddedNewLineWithEnter;

	public bool IsFieldSelected => ParentComponent.SelectedField == FieldIndex;
	public int FieldIndex => ParentComponent.InputFields.IndexOf(inputField);

	private void Awake()
	{
		hasAddedNewLineWithEnter = false;
		inputField = GetComponent<TMP_InputField>();

		inputField.onValueChanged.AddListener(OnValueChanged);
		inputField.onSubmit.AddListener(OnSubmit);
	}

	private void OnDestroy()
	{
		inputField.onValueChanged.RemoveListener(OnValueChanged);
		inputField.onSubmit.RemoveListener(OnSubmit);
	}

	private void Update()
	{
		caretPos = inputField.caretPosition;
		lengthForCaret = inputField.text.Replace("\n", "").Replace("<br>", "").Length;

		// HACK: brute force, also not checking line
		// TODO: change to new input system
		if (IsFieldSelected)
		{
			if (inputField.caretPosition <= 0 && Input.GetKeyDown(KeyCode.UpArrow))
			{
				int index = FieldIndex - 1;
				ParentComponent.SelectField(index);
			}
			else if (inputField.caretPosition >= lengthForCaret - 1 && Input.GetKeyDown(KeyCode.DownArrow))
			{
				int index = FieldIndex + 1;
				ParentComponent.SelectField(index);
			}
		}
	}


	private void OnValueChanged(string stringData)
	{
		// keep as rich text for better debugging
		inputField.text = inputField.text.Replace("\n", "<br>");
		inputField.caretPosition = lengthForCaret - 1;

		if (hasAddedNewLineWithEnter && inputField.text.Length > 4 && inputField.text.Contains("<br>"))
		{
			string newStr = inputField.text;
			newStr = newStr.Remove(inputField.text.Length - 4, 4);
			inputField.text = newStr;
		}

		hasAddedNewLineWithEnter = false;

		if (ParentComponent.NodeData != null && ParentComponent.NodeData.HasInitialized)
			ParentComponent.NodeData.Content = stringData;
	}

	private void OnSubmit(string submitValue)
	{
		// TODO: change to new input system
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				// avoids enter deselecting
				if (ParentComponent.IsSelected && Input.GetKeyDown(KeyCode.Return))
					ParentComponent.SelectField(inputField);
			}
			else
			{
				hasAddedNewLineWithEnter = true;

				TMP_InputField field = Instantiate(ParentComponent.InputFieldPrefab, ParentComponent.FunctionalNode.transform);
				ParentComponent.AddInputField(field);
			}
		}
	}
}
