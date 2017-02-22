using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

	List<TileProperties> multiSelected = new List<TileProperties>();

	public float visualizationSpeed;
	public Color emptyC, blockC, startC, endC, processedC, pathC;

	[TextArea(15, 100)]
	public string keys;

	Map map;
	BFS bfs;

	List<TileProperties> processed;
	List<TileProperties> path;

	bool visualizeRunning;

	void Start () {
		map = FindObjectOfType<Map>();
		bfs = FindObjectOfType<BFS>();
	}

	void Update () {
		// Selecting
		if (Input.GetKey(KeyCode.Mouse0)) {
			var selected = SelectTile();
			if (selected != null && !multiSelected.Contains(selected)) {
				multiSelected.Add(selected);
				ChangeColor(selected, Color.green);
			}
		}
		// Deselecting
		if (Input.GetKey(KeyCode.Mouse1)) {
			var selected = SelectTile();
			if (selected != null && multiSelected.Contains(selected)) {
				multiSelected.Remove(selected);
				ColorByType(selected);
			}
		}

		// Finalize grid
		if (Input.GetKeyDown(KeyCode.Space)) {
			multiSelected.Clear();
			ResetAllColors();
			var allTiles = map.tiles;
			foreach (TileProperties t in allTiles) {
				t.SetNeighbours();
			}
			path = bfs.Search();
			processed = bfs.processed;
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			ResetAllColors();
		}

		if (Input.GetKeyDown(KeyCode.Q) && !visualizeRunning) {
			StartCoroutine(Visualize(processed, processedC));
		}

		if (Input.GetKeyDown(KeyCode.W) && !visualizeRunning) {
			StartCoroutine(Visualize(path, pathC));
		}

		// EDITING
		if (multiSelected.Count > 0) {
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				ChangeSelectedTypes(TileProperties.TileType.Empty);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) {
				ChangeSelectedTypes(TileProperties.TileType.Block);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) {
				map.SetStart(multiSelected[0]);
				ChangeSelectedTypes(TileProperties.TileType.Start);
			}
			if (Input.GetKeyDown(KeyCode.Alpha4)) {
				map.SetEnd(multiSelected[0]);
				ChangeSelectedTypes(TileProperties.TileType.End);
			}
		}
	}

	IEnumerator Visualize (List<TileProperties> p, Color c) {
		visualizeRunning = true;
		while (p.Count > 0) {
			if (p[0].type == TileProperties.TileType.Empty) {
				ChangeColor(p[0], c);
			}
			p.RemoveAt(0);
			yield return new WaitForSeconds(visualizationSpeed);
		}
		visualizeRunning = false;
		yield return null;
	}

	void ChangeSelectedTypes (TileProperties.TileType type) {
		foreach (TileProperties t in multiSelected) {
			t.SetTileType(type);
			ColorByType(t);
		}
		multiSelected.Clear();
	}

	TileProperties SelectTile () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		TileProperties tile = null;
		if (Physics.Raycast(ray, out hit)) {
			tile = hit.transform.GetComponentInParent<TileProperties>();
		}
		return tile;
	}

	void ChangeColor (TileProperties tile, Color c) {
		var renderer = tile.GetComponentInChildren<MeshRenderer>();
		renderer.material.color = c;
	}

	void ColorByType (TileProperties tile) {
		var renderer = tile.GetComponentInChildren<MeshRenderer>();
		if (tile.type == TileProperties.TileType.Empty) {
			renderer.material.color = emptyC;
		} else if (tile.type == TileProperties.TileType.Block) {
			renderer.material.color = blockC;
		} else if (tile.type == TileProperties.TileType.Start) {
			renderer.material.color = startC;
		} else if (tile.type == TileProperties.TileType.End) {
			renderer.material.color = endC;
		}
	}

	void ResetAllColors () {
		var tiles = map.tiles;
		foreach (TileProperties t in tiles) {
			ColorByType(t);
		}
	}

	List<TileProperties> GetNeighbours (TileProperties tile, int iterations, List<TileProperties> n) {
		if (iterations == 0) {
			return n;
		}
		foreach(TileProperties t in tile.neighbours) {
			n.Add(t);
			n = GetNeighbours(t, iterations - 1, n);
		}
		return n;
	}

	List<TileProperties> DeleteDuplicates (List<TileProperties> input) {
		var output = new List<TileProperties>();
		foreach (TileProperties t in input) {
			if (!ContainsTile(output, t)) {
				output.Add(t);
			}
		}
		return output;
	}

	bool ContainsTile (List<TileProperties> input, TileProperties tile) {
		foreach (TileProperties t in input) {
			if (t.index == tile.index) {
				return true;
			}
		}
		return false;
	}
}
