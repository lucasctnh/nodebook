using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class TextNodeInputField : RequiresParent<TextNode>
{
	[Header("Debug: TextField")]
	[SerializeField, ReadOnly] protected ExternalGraphics externalGraphics;
	[SerializeField, ReadOnly] protected TMP_InputField inputField;
	[SerializeField, ReadOnly] protected int caretPos;
	[SerializeField, ReadOnly] protected int lengthForCaret;
	[SerializeField, ReadOnly] protected int textLength;
	[SerializeField, ReadOnly] protected bool hasAddedNewLineWithEnter;

	public TMP_InputField InputField
	{
		get
		{
			if (inputField == null)
				inputField = GetComponent<TMP_InputField>();
			
			return inputField;
		}
	}

	public bool IsFieldSelected => ParentComponent.SelectedField == FieldIndex;
	public int FieldIndex => ParentComponent.InputFields.IndexOf(this);

	// HACK: this is used for TodoInputField, but shouldnt be here
	public virtual bool IsToggled => false;

	#region Unity Messages
	protected virtual void Awake()
	{
		externalGraphics = GetComponent<ExternalGraphics>();

		hasAddedNewLineWithEnter = false;
		inputField = GetComponent<TMP_InputField>();
		inputField.interactable = ParentComponent.IsSelected;

		inputField.onValueChanged.AddListener(OnValueChanged);
		inputField.onSubmit.AddListener(OnSubmit);
	}

	protected virtual void OnDestroy()
	{
		inputField.onValueChanged.RemoveListener(OnValueChanged);
		inputField.onSubmit.RemoveListener(OnSubmit);
	}

	protected virtual void Update()
	{
		// basically for debug
		caretPos = inputField.caretPosition;
		lengthForCaret = inputField.text.Replace("\n", "").Replace("<br>", "").Length;

		HandleNavigationThroughFields();
		HandleBulletPoint();
	}
	#endregion

	#region Public Methods
	public virtual void HandleLoadData(string data)
	{
		inputField.text = data;
	}
	#endregion

	#region Field Events
	protected virtual void OnValueChanged(string stringData)
	{
		// keep as rich text for better debugging
		inputField.text = inputField.text.Replace("\n", "<br>"); // enter
		inputField.text = inputField.text.Replace("\v", "<br>"); // shift+enter

		inputField.caretPosition = lengthForCaret - 1;
		textLength = inputField.text.Length;

		if (hasAddedNewLineWithEnter && textLength > 4 && inputField.text[textLength - 4] == '<' && inputField.text[textLength - 1] == '>')
		{
			string newStr = inputField.text;
			newStr = newStr.Remove(textLength - 4, 4);
			inputField.text = newStr;
		}

		hasAddedNewLineWithEnter = false;

		SaveData(stringData);
	}

	protected virtual void OnSubmit(string submitValue)
	{
		// TODO: change to new input system
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				// avoids enter deselecting
				if (ParentComponent.IsSelected && Input.GetKeyDown(KeyCode.Return))
					ParentComponent.SelectField(this);
			}
			else
			{
				hasAddedNewLineWithEnter = true;

				TextNodeInputField field = Instantiate(ParentComponent.InputFieldPrefab, ParentComponent.FunctionalNode.transform);
				ParentComponent.AddInputField(field);
			}
		}
	}
	#endregion

	#region Protected Methods
	protected virtual void SaveData(string data)
	{
		if (ParentComponent.NodeData != null && ParentComponent.NodeData.HasInitialized)
		{
			// using a temporary array because save data will only be saved automatically
			// when actually changing the array values, not its children
			string[] tempContent = new string[ParentComponent.InputFields.Count];
			for (int i = 0; i < ParentComponent.InputFields.Count; i++)
				tempContent[i] = ParentComponent.InputFields[i].InputField.text;
			tempContent[FieldIndex] = data;
			ParentComponent.NodeData.Content = tempContent;
		}
	}

	protected virtual void HandleNavigationThroughFields()
	{
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

	protected virtual void HandleBulletPoint()
	{
		if (inputField.text == null || inputField.text.Length < 2) return;

		if (inputField.text[0] == '-' && inputField.text[1] == ' ')
			inputField.text = "<style=\"Bullet\">";

		inputField.caretPosition = inputField.text.Length - 1;
	}
	#endregion
}
