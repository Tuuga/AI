using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Grid : MonoBehaviour {

	public static List<Node> nodes { get; private set; }
	public Vector2 size;
	public static Vector2 mapSize;
	public static Node start { get; private set; }
	public static Node end { get; private set; }

	void Awake () {
		mapSize = size;
		BuildGrid();
	}

	void BuildGrid () {
		Camera.main.transform.position = new Vector3((mapSize.x - 1) / 2, Mathf.Max(mapSize.x, mapSize.y), (mapSize.y - 1) / 2);
		nodes = new List<Node>();
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				var coord = new Vector2(x, y);
				var pos = new Vector3(x, 0, y);
				var g = GameObject.CreatePrimitive(PrimitiveType.Quad);
				g.transform.rotation = Quaternion.Euler(90, 0, 0);
				nodes.Add(new Node(coord, pos, g));
			}
		}

		foreach (Node n in nodes) {
			n.FindNeighbours();
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

	public static void SetStart (Node n) {
		start = n;
	}

	public static void SetEnd (Node n) {
		end = n;
	}
}