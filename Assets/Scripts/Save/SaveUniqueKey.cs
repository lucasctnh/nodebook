using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[Serializable]
public struct SaveUniqueKey
{
	[SerializeField] private string uniqueKey;
	private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&()-";
	public string UniqueKey => uniqueKey;

	public static implicit operator string(SaveUniqueKey saveKey)
	{
		return saveKey.UniqueKey;
	}

	public static string GenerateKey(string source = null, string preffix = null)
	{
		int seed = (source == null || source == string.Empty) ? 0 : source.GetHashCode();
		Random random = new Random(seed);

		string key = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
		key = (preffix == null || preffix == string.Empty) ? key : $"{preffix}_{key}";

		return key;
	}
}
