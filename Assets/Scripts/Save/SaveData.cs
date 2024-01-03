using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public interface ISaveData
{
	public string Id { get; }
	public bool HasInitialized { get; }
}

[Serializable]
public class NodeData : ISaveData
{
	public delegate void NodeEvent();
	public static event NodeEvent OnRegeneratedId;

	[SerializeField] protected string id;
	[SerializeField] protected bool hasInitialized;
	[SerializeField] protected string parentCanvasId;
	[SerializeField] protected string name;
	[SerializeField] protected string[] content;
	[SerializeField] protected Vector2 position;
	[SerializeField] protected NodeType type;
	[SerializeField] protected string[] nodes;

	public virtual string Id { get => id; set { id = value; SaveManager.Save(this); } }
	public virtual bool HasInitialized { get => hasInitialized; set { hasInitialized = value; SaveManager.Save(this); } }
	public virtual string ParentCanvasId { get => parentCanvasId; set { parentCanvasId = value; SaveManager.Save(this); } }
	public virtual string Name { get => name; set { name = value; SaveManager.Save(this); } }
	public virtual string[] Content { get => content; set { content = value; SaveManager.Save(this); } }
	public virtual Vector2 AnchoredPosition { get => position; set { position = value; SaveManager.Save(this); } }
	public virtual NodeType Type { get => type; set { type = value; SaveManager.Save(this); } }
	public virtual string[] Nodes { get => nodes; set { nodes = value; SaveManager.Save(this); } }

	public NodeData() { }
	public NodeData(NodeType type, Vector2 anchoredPosition, string parentCanvasId)
	{
		Type = type;
		AnchoredPosition = anchoredPosition;
		ParentCanvasId = parentCanvasId;

		Id = SaveUniqueKey.GenerateKey($"{ParentCanvasId}_{AnchoredPosition}_{Type}", Type.ToString());
		HasInitialized = true;
	}
	public NodeData(NodeType type, Vector2 anchoredPosition, string parentCanvasId, string name)
	{
		Type = type;
		AnchoredPosition = anchoredPosition;
		ParentCanvasId = parentCanvasId;
		Name = name;

		Id = SaveUniqueKey.GenerateKey($"{ParentCanvasId}_{AnchoredPosition}_{Type}", Type.ToString());
		HasInitialized = true;
	}

	public void RegenerateId()
	{
		SaveManager.Delete(this);

		Id = SaveUniqueKey.GenerateKey($"{ParentCanvasId}_{AnchoredPosition}_{Type}", Type.ToString());

		OnRegeneratedId?.Invoke();
	}
}


[Serializable]
public class HomeData : NodeData
{
	private const string Secret = "Aww shit, here we go again";
	private const string ConstName = "Home";

	public static string IdStatic => SaveUniqueKey.GenerateKey($"{ConstName}_{Secret}", NodeType.Canvas.ToString());
	public override string Id => id = IdStatic;
	public override string Name => name = ConstName;

	public HomeData()
	{
		id = IdStatic;
		name = ConstName;
	}
}
