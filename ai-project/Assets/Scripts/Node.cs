using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public enum NodeType { Empty, Block, Start, End }
	public NodeType type { get; private set; }
	
	public Vector2 coordinates { get; private set; }
	public Vector3 position { get; private set; }
	public List<Node> neighbours { get; private set; }
	public List<Connection> connections { get; private set; }
	public GameObject visual { get; private set; }

	public Node parent;

	public int gCost;
	public int hCost;
	public int fCost { get { return gCost + hCost; } }

	public Node (Vector2 _coord, Vector3 _pos, GameObject _visual = null) {
		coordinates = _coord;
		position = _pos;
		visual = _visual;
		if (visual != null) {
			visual.transform.position = _pos;
		}
	}

	public Node () { }

	public void SetCoordinates (int x, int y) {
		coordinates = new Vector2(x, y);
	}

	public void SetTileType (NodeType newType) {
		type = newType;
	}

	public void FindNeighbours (bool diagonal) {
		neighbours = GetNeighbours(diagonal);
	}

	public void FindConnections () {
		connections = Grid.GetConnections(this);
	}

	List<Node> GetNeighbours (bool diagonal) {
		var n = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;
				if(!diagonal && Mathf.Abs(x + y) != 1) {
					continue;
				}

				int checkX = (int)coordinates.x + x;
				int checkY = (int)coordinates.y + y;

				if (checkX >= 0 && checkX < Grid.mapSize.x && checkY >= 0 && checkY < Grid.mapSize.y) {
					n.Add(Grid.GetNode(checkX, checkY));
				}
			}
		}

		//int x = (int)coordinates.x;
		//int y = (int)coordinates.y;

		//if (x + 1 < Grid.mapSize.x) {
		//	var neighbour = Grid.GetNode(x + 1, y);
		//	if (neighbour != null) {
		//		n.Add(neighbour);
		//	}
		//}
		//if (y + 1 < Grid.mapSize.y) {
		//	var neighbour = Grid.GetNode(x, y + 1);
		//	if (neighbour != null) {
		//		n.Add(neighbour);
		//	}
		//}
		//if (x - 1 >= 0) {
		//	var neighbour = Grid.GetNode(x - 1, y);
		//	if (neighbour != null) {
		//		n.Add(neighbour);
		//	}
		//}
		//if (y - 1 >= 0) {
		//	var neighbour = Grid.GetNode(x, y - 1);
		//	if (neighbour != null) {
		//		n.Add(neighbour);
		//	}
		//}

		return n;
	}
}
