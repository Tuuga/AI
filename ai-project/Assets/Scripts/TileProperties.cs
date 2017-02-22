using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour {

	public enum TileType { Empty, Block, Start, End }
	public TileType type { get; private set; }

	public Vector2 coordinates;
	public int index;
	public List<TileProperties> neighbours { get; private set; }
	public List<int> intNeighbours { get; private set; }

	Map map;

	void Start () {
		map = FindObjectOfType<Map>();
		SetNeighbours();
	}

	public void SetNeighbours () {
		neighbours = GetNeighbours();

		intNeighbours = new List<int>();
		foreach (TileProperties t in neighbours) {
			intNeighbours.Add(t.index);
		}
	}

	public void SetCoordinates (int x, int y) {
		coordinates = new Vector2(x, y);
	}

	public void SetTileType (TileType newType) {
		type = newType;
	}

	List<TileProperties> GetNeighbours () {
		var n = new List<TileProperties>();

		int x = (int)coordinates.x;
		int y = (int)coordinates.y;

		if (x + 1 < map.mapSize.x) {
			var neighbour = map.GetTileProperties(x + 1, y);
			if (neighbour != null && neighbour.type != TileType.Block) {
				n.Add(neighbour);
			}
		}
		if (y + 1 < map.mapSize.y) {
			var neighbour = map.GetTileProperties(x, y + 1);
			if (neighbour != null && neighbour.type != TileType.Block) {
				n.Add(neighbour);
			}
		}
		if (x - 1 >= 0) {
			var neighbour = map.GetTileProperties(x - 1, y);
			if (neighbour != null && neighbour.type != TileType.Block) {
				n.Add(neighbour);
			}
		}
		if (y - 1 >= 0) {
			var neighbour = map.GetTileProperties(x, y - 1);
			if (neighbour != null && neighbour.type != TileType.Block) {
				n.Add(neighbour);
			}
		}

		return n;
	}
}
