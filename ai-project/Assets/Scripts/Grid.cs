﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	public bool showBlockZones;
	public Material blockZoneMat;
	public float nodeBlockRadius;
	public LayerMask mask;
	public Material mat;
	public bool instantiateVisuals;
	public PrimitiveType nodeVisualType;
	public Vector3 nodeVisualRotation;
	public Vector3 nodeVisualScale;

	public static List<Node> nodes { get; private set; }
	public static List<Node> validNodes { get; private set; }
	[Space()]
	public Vector2 size;

	public bool generateRandom;
	public float percentToBlock;

	public bool diagonalNeighbours;
	public static Vector2 mapSize;

	public int MaxSize {
		get {
			return (int)(size.x * size.y);
		}
	}

	void Awake () {
		mapSize = size;
		BuildGrid();
		EvaluateBlockingNodes();
		SetValidNodes();
		
		if (generateRandom) {
			foreach (Node n in nodes) {
				if (Random.Range(0, 100f) <= percentToBlock) {
					n.SetNodeType(Node.NodeType.Block);
				}
			}
		}
	}

	void BuildGrid () {
		Camera.main.transform.position = new Vector3((mapSize.x - 1) / 2, Mathf.Max(mapSize.x, mapSize.y), (mapSize.y - 1) / 2);
		nodes = new List<Node>();
		var visualParent = new GameObject("Grid Visuals");
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				var coord = new Vector2(x, y);
				var pos = new Vector3(x, 0, y);

				if (instantiateVisuals) {
					var g = GameObject.CreatePrimitive(nodeVisualType);
					g.transform.SetParent(visualParent.transform);
					g.name = string.Format("Node ({0}, {1})", x, y);
					g.transform.localScale = nodeVisualScale;
					g.transform.rotation = Quaternion.Euler(90, 0, 0);
					g.GetComponent<MeshRenderer>().material = mat;
					nodes.Add(new Node(coord, pos, g));
				} else {
					nodes.Add(new Node(coord, pos));
				}

			}
		}

		foreach (Node n in nodes) {
			n.FindNeighbours(diagonalNeighbours);
		}
	}

	public static Node GetNodeWorldPoint (Vector3 point) {
		float px = point.x / mapSize.x;
		float py = point.z / mapSize.y;
		px = Mathf.Clamp01(px);
		py = Mathf.Clamp01(py);

		var x = Mathf.RoundToInt((mapSize.x) * px);
		var y = Mathf.RoundToInt((mapSize.y) * py);
		
		return GetNode(x, y);
	}

	public static void SetMapSize (Vector2 s) {
		mapSize = s;
	}

	public static Node GetNode (int x, int y) {
		if (x >= mapSize.x || y >= mapSize.y || x < 0 || y < 0) {
			return null;
		}
		return nodes[(int)(y * mapSize.y + x)];
	}

	public static int GetNodeIndex (int x, int y) {
		return (int)(y * mapSize.y + x);
	}

	void SetValidNodes () {
		validNodes = new List<Node>();
		foreach (Node n in nodes) {
			if (n.type == Node.NodeType.Empty) {
				validNodes.Add(n);
			}
		}
	}

	void EvaluateBlockingNodes () {
		foreach (Node n in nodes) {
			var colliders = Physics.OverlapSphere(n.position, nodeBlockRadius, mask);
			if (colliders.Length > 0) {
				n.SetNodeType(Node.NodeType.Block);
				if (showBlockZones) {
					var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					g.transform.localScale = new Vector3(nodeBlockRadius, nodeBlockRadius, nodeBlockRadius);
					g.GetComponent<MeshRenderer>().material = blockZoneMat;
					g.transform.position = n.position;
				}				
			}
		}
	}
}