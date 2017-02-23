using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS : MonoBehaviour {

	public List<Node> processed { get; private set; }

	public List<Node> Search () {
		processed = new List<Node>();
		var s = new Stack<Node>();

		var discovered = new Dictionary<Node, Node>();
		discovered.Add(Grid.start, null);
		var disc = new List<Node>();

		s.Push(Grid.start);
		while (s.Count > 0) {
			var v = s.Pop();

			if (v.type == Node.NodeType.End) {
				var path = new List<Node>();
				path.Add(v);
				var lastNode = discovered[v];
				while (lastNode != null) {
					path.Add(lastNode);
					lastNode = discovered[lastNode];
				}
				path.Reverse();
				return path;
			}

			if (!disc.Contains(v) && v.type != Node.NodeType.Block) {
				disc.Add(v);
				foreach (Node n in v.neighbours) {
					s.Push(n);
					if (!discovered.ContainsKey(n) && n.type != Node.NodeType.Block) {
						discovered.Add(n, v);
						processed.Add(n);
					}
				}
			}
		}
		s.Clear();
		return new List<Node>();
	}
}
