using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BFS : MonoBehaviour {

	public List<Node> processed { get; private set; }

	public List<Node> Search (Node start, Node end) {
		var sw = new Stopwatch();
		sw.Start();

		processed = new List<Node>();
		var q = new Queue<Node>();
		q.Enqueue(start);

		var discovered = new Dictionary<Node, Node>();
		discovered.Add(start, null);

		while (q.Count > 0) {
			var v = q.Dequeue();
			
			if (v == end) {
				var path = new List<Node>();
				path.Add(v);
				var lastNode = discovered[v];
				while (lastNode != null) {
					path.Add(lastNode);
					lastNode = discovered[lastNode];
				}
				path.Reverse();
				sw.Stop();
				print(sw.Elapsed);
				return path;
			}
			foreach (Node t in v.neighbours) {
				if (!discovered.ContainsKey(t) && t.type != Node.NodeType.Block) {
					q.Enqueue(t);
					discovered.Add(t, v);
					processed.Add(t);
				}
			}
		}
		return new List<Node>();
	}
}
