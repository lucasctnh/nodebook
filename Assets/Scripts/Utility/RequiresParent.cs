using NaughtyAttributes;
using UnityEngine;

public abstract class RequiresParent<T> : MonoBehaviour where T : MonoBehaviour
{
	[SerializeField, ReadOnly] private T parentComponent;

	protected T ParentComponent => parentComponent == null
		? parentComponent = GetComponentInParent<T>(true)
		: parentComponent;
}
