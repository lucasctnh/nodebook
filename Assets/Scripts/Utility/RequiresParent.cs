using NaughtyAttributes;
using UnityEngine;

public abstract class RequiresParent<T> : MonoBehaviour where T : MonoBehaviour
{
	[Header("Debug: Parent")]
	[SerializeField, ReadOnly] private T parentComponent;

	protected T ParentComponent => parentComponent == null
		? parentComponent = GetComponentInParent<T>(true)
		: parentComponent;
}
