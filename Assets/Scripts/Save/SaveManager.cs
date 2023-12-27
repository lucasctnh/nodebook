using Leguar.TotalJSON;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
	[Header("Debug")]
	[SerializeField, ReadOnly] private bool hasHomeData;

	#region Public Methods
	public static void Save(ISaveData data)
	{
		if (IsInstanceValid == false) return;
		if (Application.isPlaying == false) return;

		if (data.HasInitialized == false)
		{
			Debug.LogError("Trying to save data that hasnt initialized.");
			return;
		}

		if (data.Id == null || data.Id == string.Empty)
		{
			Debug.LogError("Trying to save data with invalid id.");
			return;
		}

		JSON json = JSON.Serialize(data);

		json.DebugInEditor("Last Saved Data");

		Instance.SaveInternal(json, data.Id);
	}

	public static T Load<T>(ISaveData data)
	{
		return Load<T>(data.Id);
	}

	public static T Load<T>(string id)
	{
		if (IsInstanceValid == false) return default;
		if (Application.isPlaying == false) return default;

		string jsonAsString = Instance.LoadInternal(id)?.CreateString();

		if (jsonAsString == null || jsonAsString == string.Empty || jsonAsString == default)
		{
			Debug.LogError($"No save file was found for {id}");
			return default;
		}

		JSON json = JSON.ParseString(jsonAsString);

		json.DebugInEditor("Last Loaded Data");

		return json.Deserialize<T>();
	}

	public static void Delete(ISaveData data)
	{
		string filePath = Path.Combine(Application.persistentDataPath, $"{data.Id}.json");

		if (!File.Exists(filePath))
		{
			Debug.LogError($"Could not find save file for {data.Id} in {filePath}");
			return;
		}
		else
		{
			File.Delete(filePath);
		}
	}
	#endregion

	#region Private Methods
	private void SaveInternal(JSON jsonObject, string id)
	{
		string jsonAsString = jsonObject.CreateString();
		string filePath = Path.Combine(Application.persistentDataPath, $"{id}.json");

		StreamWriter writer;
		try
		{
			writer = new StreamWriter(filePath);
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Could not save file for {id} in {filePath}, error: {e}");
			return;
		}

		writer.WriteLine(jsonAsString);
		writer.Close();
	}

	private JSON LoadInternal(string id)
	{
		if (id == null || id == string.Empty)
		{
			Debug.LogError("Trying to load with invalid id.");
			return null;
		}

		string filePath = Path.Combine(Application.persistentDataPath, $"{id}.json");
		StreamReader reader;
		try
		{
			reader = new StreamReader(filePath);
		}
		catch (System.Exception e)
		{
			Debug.LogError($"No save file was found for {id} in {filePath}, error: {e}");
			return null;
		}

		string jsonAsString = reader.ReadToEnd();
		reader.Close();

		if (jsonAsString == null || jsonAsString == string.Empty)
		{
			Debug.LogError($"Save file is empty for {id}");
			return null;
		}

		JSON jsonObject = JSON.ParseString(jsonAsString);

		return jsonObject;
	}
	#endregion
}
