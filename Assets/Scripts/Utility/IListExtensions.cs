using System.Collections.Generic;

public static class IListExtensions
{
	/// <summary>
	/// Adds to list only if the element doesn't exist there yet.
	/// </summary>
	/// <returns>True if an element was added, false otherwise</returns>
	public static bool AddIfNew<T>(this IList<T> list, T newElement)
	{
		if (list == null) return false;
		if (list.Contains(newElement)) return false;

		list.Add(newElement);
		return true;
	}

	/// <summary>
	/// Adds to list all the elements that dont exist there yet.
	/// </summary>
	/// <returns>True if an element was added, false otherwise</returns>
	public static bool AddIfNew<T>(this IList<T> list, IList<T> newElements)
	{
		if (list == null) return false;
		if (newElements == null) return false;
		if (newElements.Count <= 0) return false;

		bool response = false;
		foreach (T newElement in newElements)
		{
			if (newElement == null) continue;
			response |= list.AddIfNew(newElement);
		}
		return response;
	}
}
