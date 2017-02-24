using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

	public List<Node> processed { get; private set; }

	public List<Node> Search () {
		processed = new List<Node>();

		var startNode = Grid.start;
		var targetNode = Grid.end;

		List<Node> open = new List<Node>();
		HashSet<Node> closed = new HashSet<Node>();
		open.Add(startNode);
		processed.Add(startNode);

		while (open.Count > 0) {
			Node currentNode = open[0];
			for (int i = 1; i < open.Count; i++) {
				if (open[i].fCost < currentNode.fCost || open[i].fCost == currentNode.fCost && open[i].hCost < currentNode.hCost) {
					currentNode = open[i];
				}
			}

			open.Remove(currentNode);
			closed.Add(currentNode);

			if (currentNode == targetNode) {
				List<Node> path = new List<Node>();
				Node current = targetNode;

				while (current != startNode) {
					path.Add(current);
					current = current.parent;
				}
				path.Reverse();
				return path;
			}

			foreach (Node neighbour in currentNode.neighbours) {
				if (neighbour.type == Node.NodeType.Block || closed.Contains(neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !open.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
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
