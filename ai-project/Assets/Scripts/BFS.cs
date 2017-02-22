using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour {

	Map map;
	public List<TileProperties> processed { get; private set; }

	void Start () {
		map = FindObjectOfType<Map>();
	}

	public List<TileProperties> Search () {
		processed = new List<TileProperties>();
		var q = new Queue<TileProperties>();
		q.Enqueue(map.start);

		var discovered = new Dictionary<TileProperties, TileProperties>();
		discovered.Add(map.start, null);

		while (q.Count > 0) {
			var v = q.Dequeue();
			
			if (v.type == TileProperties.TileType.End) {
				var path = new List<TileProperties>();
				path.Add(v);
				var lastTile = discovered[v];
				while (lastTile != null) {
					path.Add(lastTile);
					lastTile = discovered[lastTile];
				}
				path.Reverse();
				return path;
			}
			foreach (TileProperties t in v.neighbours) {
				if (!discovered.ContainsKey(t)) {
					q.Enqueue(t);
					discovered.Add(t, v);
					processed.Add(t);
				}
			}
		}
		return null;
	}
}
