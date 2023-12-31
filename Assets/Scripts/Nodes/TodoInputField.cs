using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TodoInputField : TextNodeInputField
{
	[Header("Settings: Todo Field")]
	[SerializeField] private Color defaultTextColor;
	[SerializeField] private Color markedTextColor;
	[Header("Debug: TodoField")]
	[SerializeField, ReadOnly] private Button toggleButton;
	[SerializeField, ReadOnly] private Image toggleIcon;
	[SerializeField, ReadOnly] private bool isToggled;

	public override bool IsToggled => isToggled;

	#region Unity Messages
	protected override void Awake()
	{
		base.Awake();
		toggleButton = GetComponentInChildren<Button>(true);
		toggleIcon = toggleButton.transform.GetChild(0).GetComponent<Image>();

		toggleButton.onClick.AddListener(OnToggle);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		toggleButton.onClick.RemoveListener(OnToggle);
	}

	protected override void Update()
	{
		base.Update();
		if (toggleButton != null)
			toggleButton.interactable = ParentComponent.IsSelected;
	}
	#endregion

	#region Public Methods
	public override void HandleLoadData(string data)
	{
		if (data.All(x => x == '['))
		{
			Debug.LogError("Found more than one bracket, this is not allowed.");
			return;
		}

		if (data[0] != '[')
		{
			// assume is not toggled
			isToggled = false;
		}

		if (data.Contains("[x]"))
			isToggled = true;
		else if (data.Contains("[]"))
			isToggled = false;

		string justContent = isToggled ? data.Remove(0, 4) : data.Remove(0, 3);
		inputField.text = justContent;

		UpdateUI();
	}
	#endregion

	#region Protected Methods
	protected override void SaveData(string data)
	{
		if (ParentComponent.NodeData != null && ParentComponent.NodeData.HasInitialized)
		{
			// using a temporary array because save data will only be saved automatically
			// when actually changing the array values, not its children
			string[] tempContent = new string[ParentComponent.InputFields.Count];
			for (int i = 0; i < ParentComponent.InputFields.Count; i++)
				tempContent[i] = (ParentComponent.InputFields[i].IsToggled ? "[x] " : "[] ") + ParentComponent.InputFields[i].InputField.text;

			tempContent[FieldIndex] = (isToggled ? "[x] " : "[] ") + data;
			ParentComponent.NodeData.Content = tempContent;
		}
	}
	#endregion

	#region Private Methods
	private void OnToggle()
	{
		if (!ParentComponent.IsSelected) return;

		isToggled = !isToggled;
		UpdateUI();

		SaveData(inputField.text);
	}

	private void UpdateUI()
	{
		if (toggleButton == null)
			toggleButton = GetComponentInChildren<Button>(true);
		if (toggleIcon == null)
			toggleIcon = toggleButton.transform.GetChild(0).GetComponent<Image>();

		toggleIcon.gameObject.SetActive(isToggled);
		inputField.textComponent.color = isToggled ? markedTextColor : defaultTextColor;
	}
	#endregion
}
