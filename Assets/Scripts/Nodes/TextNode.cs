using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TextNode : Node
{
	[Header("References: TextNode")]
	[SerializeField] protected TextNodeInputField inputFieldPrefab;
	[Header("Debug: TextNode")]
	[SerializeField, ReadOnly] protected List<TextNodeInputField> inputFields = new List<TextNodeInputField>();
	[SerializeField, ReadOnly] protected int selectedField;

	public override NodeType NodeType => NodeType.Text;
	public virtual List<TextNodeInputField> InputFields => inputFields;
	public TextNodeInputField InputFieldPrefab => inputFieldPrefab;
	public int SelectedField => selectedField;

	protected override void Awake()
	{
		base.Awake();
		selectedField = 0;
		inputFields.AddIfNew(GetComponentInChildren<TextNodeInputField>(true));

		DeselectNode();
	}

	protected virtual void Update()
	{
		float greaterHeight = 0f;
		foreach (RectTransform child in rectTransform)
		{
			if (child.rect.height > greaterHeight)
				greaterHeight = child.rect.height;
		}

		rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, greaterHeight);
	}

	public override void SelectNode()
	{
		if (isSelected) return;

		base.SelectNode();
		foreach (var field in inputFields)
			field.InputField.interactable = true;

		if (inputFields[0].InputField.text == string.Empty && inputFields.Count <= 1)
			inputFields[0].InputField.Select();
	}

	public override void DeselectNode()
	{
		base.DeselectNode();
        foreach (var field in inputFields)
			field.InputField.interactable = false;
	}

	public void AddInputField(TextNodeInputField field)
	{
		inputFields.AddIfNew(field);
		ExternalGraphics external = field.GetComponent<ExternalGraphics>();
		if (external)
			externalGraphics.AddIfNew(external);
		
		UnsubscribeToExternalEvents();
		UnsubscribeToExternalSelect();

		SubscribeToExternalEvents();
		SubscribeToExternalSelect();

		SelectField(field);
	}

	public void SelectField(TextNodeInputField field)
	{
		field.InputField.ActivateInputField();
		selectedField = inputFields.IndexOf(field);
	}

	public void SelectField(int index)
	{
		if (index < 0)
			index = 0;
		if (index >= inputFields.Count)
			index = inputFields.Count - 1;

		inputFields[index].InputField.ActivateInputField();
		selectedField = index;
	}

	protected override void HandleLoadData()
	{
		if (nodeData.Content == null) return;
		if (nodeData.Content.Length <= 0) return;

		inputFields[0].HandleLoadData(nodeData.Content[0]);

		if (nodeData.Content.Length > 1)
		{
			for (int i = 1; i < nodeData.Content.Length; i++)
			{
				TextNodeInputField field = Instantiate(InputFieldPrefab, FunctionalNode.transform);
				AddInputField(field);
				field.HandleLoadData(nodeData.Content[i]);
			}
		}
	}
}
