using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class Map : MonoBehaviour {

	public List<TileProperties> tiles; // { get; private set; }
	public Vector2 mapSize;
	public TileProperties start { get; private set; }
	public TileProperties end { get; private set; }


	void Awake () {
		var foundTiles = FindObjectsOfType<TileProperties>();

		tiles = new List<TileProperties>(foundTiles);
		foreach (TileProperties t in foundTiles) {
			tiles[t.index] = t;
		}
	}

	public void SetMapSize (Vector2 size) {
		mapSize = size;
	}

	public TileProperties GetTileProperties (int x, int y) {
		if (x >= mapSize.x || y >= mapSize.y || x < 0 || y < 0) {
			return null;
		}
		return tiles[(int)(y * mapSize.y + x)];
	}

	public void SetStart (TileProperties tile) {
		start = tile;
	}

	public void SetEnd (TileProperties tile) {
		end = tile;
	}
}