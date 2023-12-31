using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TodoNode : TextNode
{
	public override NodeType NodeType => NodeType.Todo;
}
