using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS : MonoBehaviour {

	public List<TileProperties> processed { get; private set; }

	Map map;

	void Start () {
		map = FindObjectOfType<Map>();
	}

	public List<TileProperties> Search () {
		processed = new List<TileProperties>();
		var s = new Stack<TileProperties>();

		var discovered = new Dictionary<TileProperties, TileProperties>();
		discovered.Add(map.start, null);
		var disc = new List<TileProperties>();

		s.Push(map.start);
		while (s.Count > 0) {
			var v = s.Pop();

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

			if (!disc.Contains(v)) {
				disc.Add(v);
				foreach (TileProperties t in v.neighbours) {
					s.Push(t);
					if (!discovered.ContainsKey(t)) {
						discovered.Add(t, v);
						processed.Add(t);
					}					
				}
			}
		}
		s.Clear();
		return null;
	}
}
