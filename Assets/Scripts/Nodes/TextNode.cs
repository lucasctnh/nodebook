using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TextNode : Node
{
	[Header("References: TextNode")]
	[SerializeField] private TMP_InputField inputFieldPrefab;
	[Header("Debug: TextNode")]
	[SerializeField, ReadOnly] private List<TMP_InputField> inputFields = new List<TMP_InputField>();
	[SerializeField, ReadOnly] private int selectedField;

	public override NodeType NodeType => NodeType.Text;
	public List<TMP_InputField> InputFields => inputFields;
	public TMP_InputField InputFieldPrefab => inputFieldPrefab;
	public int SelectedField => selectedField;

	protected override void Awake()
	{
		base.Awake();
		selectedField = 0;
		inputFields.Add(GetComponentInChildren<TMP_InputField>(true));

		DeselectNode();
	}

	public override void SelectNode()
	{
		if (isSelected) return;

		base.SelectNode();
		foreach (var field in inputFields)
			field.interactable = true;

		if (inputFields[0].text == string.Empty && inputFields.Count <= 1)
			inputFields[0].Select();
	}

	public override void DeselectNode()
	{
		base.DeselectNode();
        foreach (var field in inputFields)
			field.interactable = false;
	}

	public void AddInputField(TMP_InputField field)
	{
		inputFields.Add(field);
		SelectField(field);
	}

	public void SelectField(TMP_InputField field)
	{
		field.ActivateInputField();
		selectedField = inputFields.IndexOf(field);
	}

	public void SelectField(int index)
	{
		if (index < 0)
			index = 0;
		if (index >= inputFields.Count)
			index = inputFields.Count - 1;

		inputFields[index].ActivateInputField();
		selectedField = index;
	}

	protected override void HandleLoadData()
	{
		inputFields[0].text = nodeData.Content;
	}
}
