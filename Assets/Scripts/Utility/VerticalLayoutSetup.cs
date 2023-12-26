using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalLayoutSetup : RequiresComponent<VerticalLayoutGroup>
{
	private int waitPasses = 5;
	private bool startSetup = false;

	[Button]
	public void Setup()
	{
		RequiredComponent.enabled = true;
		startSetup = true;
		waitPasses = 5;
	}

	private void OnDrawGizmos()
	{
		if (!startSetup)
		{
			RequiredComponent.enabled = false;
			return;
		}

		if (startSetup)
		{
			waitPasses--;

			if (waitPasses <= 0)
			{
				startSetup = false;
				RequiredComponent.enabled = false;
				waitPasses = 5;
			}
        }
	}
}
