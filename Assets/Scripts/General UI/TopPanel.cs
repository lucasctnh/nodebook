using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
	public delegate void TopPanelEvent(string canvasId);
	public static event TopPanelEvent OnNavigationEvent;

	[Header("References")]
	[SerializeField] private Button homeButton;
	[SerializeField] private GameObject dividerPrefab;
	[SerializeField] private NavigationButton navButtonPrefab;
	[Header("Debug")]
	[SerializeField, ReadOnly] private List<NavigationButton> navButtons = new List<NavigationButton>();

	private void Awake()
	{
		Assert.IsNotNull(homeButton);

		CanvasNode.OnCanvasNodeSelected += OnOpenCanvas;
		PageNode.OnPageNodeSelected += OnOpenPage;
		NavigationButton.OnAnyNavigation += OnNavigate;

		homeButton.onClick.AddListener(OnNavigateHome);
	}

	private void OnDestroy()
	{
		CanvasNode.OnCanvasNodeSelected -= OnOpenCanvas;
		PageNode.OnPageNodeSelected -= OnOpenPage;
		NavigationButton.OnAnyNavigation -= OnNavigate;

		homeButton.onClick.RemoveListener(OnNavigateHome);
	}

	private void OnNavigateHome()
	{
		if (transform.childCount > 1)
		{
			for (int i = 1; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);
		}

		OnNavigationEvent?.Invoke(null);
	}

	private void OnNavigate(NavigationButton navigationButton)
	{
		List<Transform> children = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
			children.Add(transform.GetChild(i));

        int index = children.IndexOf(navigationButton.transform) + 1;
		if (transform.childCount > 1)
		{
			for (int i = index; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);
		}

		OnNavigationEvent?.Invoke(navigationButton.NodeData.Id);
	}

	private void OnOpenCanvas(CanvasNode node) => AddNavigationButton(node.NodeData);
	private void OnOpenPage(PageNode node) => AddNavigationButton(node.NodeData);

	private void AddNavigationButton(NodeData nodeData)
	{
		Instantiate(dividerPrefab, transform);
		NavigationButton navButton = Instantiate(navButtonPrefab, transform);
		navButton.Name = nodeData.Name;
		navButton.NodeData = nodeData;

        foreach (var btn in navButtons)
			btn.IsLast = false;

        navButton.IsLast = true;
		navButtons.Add(navButton);
	}
}
