using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : Singleton<NavigationManager>
{
	protected override bool Awake()
	{
		bool response = base.Awake();



		return response;
	}

	private void OnDestroy()
	{
		
	}
}
