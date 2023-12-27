using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NodesLibrary), menuName = "Nodes/Create Nodes Library")]
public class NodesLibrary : ScriptableObject
{
	[SerializeField] private SerializedDictionary<NodeType, Node> library = new SerializedDictionary<NodeType, Node>();
	public SerializedDictionary<NodeType, Node> Library => library;
}
