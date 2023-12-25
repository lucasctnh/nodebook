using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : Singleton<ViewManager>
{
	[Button]
	public static void ShowPageCanvas()
	{
		PageCanvas.Show();
		InfiniteCanvas.Hide();
	}

	[Button]
	public static void ShowInfiniteCanvas()
	{
		InfiniteCanvas.Show();
		PageCanvas.Hide();
	}
}
