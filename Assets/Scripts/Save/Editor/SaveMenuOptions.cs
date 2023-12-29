using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveMenuOptions
{
	[MenuItem("Volphin/Clear All Saves")]
	static void DeleteSaves()
	{
		PlayerPrefs.DeleteAll();

		string[] filePaths = Directory.GetFiles(Application.persistentDataPath + "/", "*.json");

		for (int i = 0; i < filePaths.Length; i++)
		{
			if (!File.Exists(filePaths[i]))
			{
				Debug.LogError($"Could not find save file {filePaths[i]}");
				return;
			}
			else
			{
				File.Delete(filePaths[i]);
			}
		}

		Debug.Log("Cleared all saves.");
	}

	[MenuItem("Volphin/Open PersistentPath")]
	static void OpenPersistentPath()
	{
		EditorUtility.RevealInFinder(Application.persistentDataPath);
	}
}
