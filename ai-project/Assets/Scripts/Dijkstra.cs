using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Connection {
	public Node from;
	public Node to;
	public float cost;

	public Connection (Node _from, Node _to, float _cost) {
		from = _from;
		to = _to;
		cost = _cost;
	}
}

public class Dijkstra : MonoBehaviour {

	public class NodeRecord {
		public Node node;
		public Connection connection;
		public float costSoFar;

		public NodeRecord (Node n, Connection c, float cost) {
			node = n;
			connection = c;
			costSoFar = cost;
		}
		public NodeRecord () {
			node = new Node();
			connection = new Connection();
			costSoFar = 0;
		}
	}

	public List<Node> processed { get; private set; }
	public List<Node> openList = new List<Node>();
	public List<Node> closedList = new List<Node>();

	public List<Node> Search () {
		processed = new List<Node>();

		var startRecord = new NodeRecord();
		startRecord.node = Grid.start;

		var open = new List<NodeRecord>();
		open.Add(startRecord);
		openList.Add(startRecord.node); // Debug

		var closed = new List<NodeRecord>();

		var current = new NodeRecord();
		while (open.Count > 0) {
			current = SmallestCost(open);

			if (current.node.type == Node.NodeType.End) { break; }

			var connections = current.node.connections;
			foreach (Connection c in connections) {
				var endNode = c.to;
				var endNodeCost = current.costSoFar + c.cost;

				if (closed.Find(node => node.node == endNode) != null) {
					continue;
				} else if (open.Find(node => node.node == endNode) != null) {

					var endNodeRecord = open.Find(node => node.node == endNode);
					if (endNodeRecord.costSoFar <= endNodeCost) { continue; }
				} else {
					var endNodeRecord = new NodeRecord();
					endNodeRecord.node = endNode;
					endNodeRecord.costSoFar = endNodeCost;
					endNodeRecord.connection = c;

					processed.Add(endNode);

					if (open.Find(node => node.node == endNode) == null) {
						open.Add(endNodeRecord);
						openList.Add(endNodeRecord.node); // Debug
					}
				}
			}
			open.Remove(current);
			closed.Add(current);
			closedList.Add(current.node);
		}

		if (current.node.type != Node.NodeType.End) {
			return new List<Node>();
		} else {
			var path = new List<Node>();
			while (current.node.type != Node.NodeType.Start) { // Doesn't end properly
				path.Add(current.connection.from);
				if (current.node == current.connection.from) {
					print("this and from is same");
					break;
				}
				current.node = current.connection.from;
				if (path.Count >= 100) {
					print("broke");
					break;
				}
			}
			path.Reverse();
			return path;
		}
	}

	bool ContainsNode (List<NodeRecord> nrs, Node n) {
		foreach (NodeRecord nr in nrs) {
			if (nr.node == n) { return true; }
		}
		return false;
	}

	NodeRecord FindByNode (List<NodeRecord> nrs, Node n) {
		foreach (NodeRecord nr in nrs) {
			if (nr.node == n) {
				return nr;
			}
		}
		return new NodeRecord();
	}

	NodeRecord SmallestCost (List<NodeRecord> r) {
		float lowest = Mathf.Infinity;
		NodeRecord smallest = new NodeRecord();
		foreach (NodeRecord nr in r) {
			if (nr.costSoFar < lowest) {
				smallest = nr;
				lowest = nr.costSoFar;
			}
		}
		return smallest;
	}
}
