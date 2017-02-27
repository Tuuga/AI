using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AStar : MonoBehaviour {

	public List<Node> processed { get; private set; }
	Grid grid;

	void Start () {
		grid = FindObjectOfType<Grid>();
	}


	public List<Node> Search (Node start, Node end) {
		var sw = new Stopwatch();
		sw.Start();

		processed = new List<Node>();

		//List<Node> open = new List<Node>();
		Heap<Node> open = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closed = new HashSet<Node>();
		open.Add(start);
		processed.Add(start);

		while (open.Count > 0) {
			Node currentNode = open.RemoveFirst();
			//Node currentNode = open[0];
			//for (int i = 1; i < open.Count; i++) {
			//	if (open[i].fCost < currentNode.fCost || open[i].fCost == currentNode.fCost && open[i].hCost < currentNode.hCost) {
			//		currentNode = open[i];
			//	}
			//}

			//open.Remove(currentNode);
			closed.Add(currentNode);

			if (currentNode == end) {
				List<Node> path = new List<Node>();
				Node current = end;

				while (current != start) {
					path.Add(current);
					current = current.parent;
				}
				path.Reverse();
				sw.Stop();
				print(sw.ElapsedMilliseconds);
				return path;
			}

			foreach (Node neighbour in currentNode.neighbours) {
				if (neighbour.type == Node.NodeType.Block || closed.Contains(neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !open.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, end);
					//neighbour.vCost = Mathf.RoundToInt(Vector3.Distance(neighbour.position, targetNode.position) * 10f);
					neighbour.parent = currentNode;
					
					if (!open.Contains(neighbour)) {
						open.Add(neighbour);
						processed.Add(neighbour);
					}
				}
			}
		}
		return new List<Node>();
	}

	int GetDistance (Node a, Node b) {
		int dstX = Mathf.Abs((int)a.coordinates.x - (int)b.coordinates.x);
		int dstY = Mathf.Abs((int)a.coordinates.y - (int)b.coordinates.y);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		}
		return 14 * dstX + 10 * (dstY - dstX);
	}
}
