using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

	List<Node> multiSelected = new List<Node>();
	public float visualizationSpeed;
	public Color emptyC, blockC, startC, endC, processedC, pathC;

	public Text processedCount, pathCount, methodType;

	[TextArea(15, 100)]
	public string keys;

	public enum SearchMethod { BFS, DFS, Dijkstra, AStar }
	public SearchMethod useMethod;

	BFS bfs;
	DFS dfs;
	Dijkstra dijkstra;
	AStar astar;

	List<Node> processed;
	List<Node> path;

	bool visualizeRunning;
	Coroutine currentVis;

	void Start () {
		bfs = FindObjectOfType<BFS>();
		dfs = FindObjectOfType<DFS>();
		dijkstra = FindObjectOfType<Dijkstra>();
		astar = FindObjectOfType<AStar>();
	}

	void Update () {
		// Selecting
		if (Input.GetKey(KeyCode.Mouse0)) {
			var selected = MouseSelect();

			if (selected != null && !multiSelected.Contains(selected)) {
				multiSelected.Add(selected);
				ChangeColor(selected, Color.green);
			}
		}

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0)) {
			var selected = MouseSelect();

			if (selected != null) {
				for (int i = 0; i < selected.neighbours.Count; i++) {
					float r = (float)i / selected.neighbours.Count;
					float g = 1 - r;
					ChangeColor(selected.neighbours[i], new Color(r, g, 0));
				}
			}
		}

		// Deselecting
		if (Input.GetKey(KeyCode.Mouse1)) {
			var selected = MouseSelect();

			if (selected != null && multiSelected.Contains(selected)) {
				multiSelected.Remove(selected);
				ColorByType(selected);
			}
		}

		// Finalize grid and search
		if (Input.GetKeyDown(KeyCode.Space)) {
			multiSelected.Clear();
			ResetAllColors();

			var nodes = Grid.nodes;
			foreach (Node n in nodes) {
				n.FindConnections();
			}

			bool canSearch = true;
			if (Grid.start == null) {
				Debug.LogError("NO START NODE");
				canSearch = false;
			}
			if (Grid.end == null) {
				Debug.LogError("NO END NODE");
				canSearch = false;
			}

			if (!canSearch) { return; }

			if (useMethod == SearchMethod.BFS) {
				path = bfs.Search();
				processed = bfs.processed;
			} else if (useMethod == SearchMethod.DFS) {
				path = dfs.Search();
				processed = dfs.processed;
			} else if (useMethod == SearchMethod.Dijkstra) {
				path = dijkstra.Search();
				processed = dijkstra.processed;
			} else if (useMethod == SearchMethod.AStar) {
				path = astar.Search();
				processed = astar.processed;
			}

			processedCount.text = "Processed: " + processed.Count;
			pathCount.text = "Path: " + path.Count;
			methodType.text = "Method: " + useMethod.ToString();
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			ResetAllColors();
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			var d = FindObjectOfType<Grid>().diagonalNeighbours;
			foreach (Node n in Grid.nodes) {
				n.FindNeighbours(d);
			}
		}
		if (Input.GetKeyDown(KeyCode.Q) && !visualizeRunning) {
			currentVis = StartCoroutine(Visualize(processed, processedC));
		}
		if (Input.GetKeyDown(KeyCode.W) && !visualizeRunning) {
			currentVis = StartCoroutine(Visualize(path, pathC));
		}
		if (Input.GetKeyDown(KeyCode.E) && visualizeRunning && currentVis != null) {
			StopCoroutine(currentVis);
			currentVis = null;
			visualizeRunning = false;
		}

		// EDITING
		if (multiSelected.Count > 0) {
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				ChangeSelectedTypes(Node.NodeType.Empty);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) {
				ChangeSelectedTypes(Node.NodeType.Block);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) {
				Grid.SetStart(multiSelected[0]);
				ChangeSelectedTypes(Node.NodeType.Start);
			}
			if (Input.GetKeyDown(KeyCode.Alpha4)) {
				Grid.SetEnd(multiSelected[0]);
				ChangeSelectedTypes(Node.NodeType.End);
			}
		}
	}

	Node MouseSelect () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit)) {
			return Grid.GetNodeWorldPoint(hit.point);
		}
		return null;
	}

	IEnumerator Visualize (List<Node> p, Color c) {
		visualizeRunning = true;
		while (p.Count > 0) {
			if (p[0].type == Node.NodeType.Empty) {
				ChangeColor(p[0], c);
			}
			p.RemoveAt(0);
			yield return new WaitForSeconds(visualizationSpeed);
		}
		visualizeRunning = false;
		currentVis = null;
		yield return null;
	}

	void ChangeSelectedTypes (Node.NodeType type) {
		foreach (Node t in multiSelected) {
			if (type != Node.NodeType.Start && t.type == Node.NodeType.Start) { Grid.SetStart(null); }
			if (type != Node.NodeType.End && t.type == Node.NodeType.End) { Grid.SetEnd(null); }

			t.SetTileType(type);
			ColorByType(t);
		}
		multiSelected.Clear();
	}

	void ChangeColor (Node n, Color c) {
		var renderer = n.visual.GetComponent<MeshRenderer>();
		renderer.material.color = c;
	}

	void ColorByType (Node n) {
		var renderer = n.visual.GetComponent<MeshRenderer>();
		if (n.type == Node.NodeType.Empty) {
			renderer.material.color = emptyC;
		} else if (n.type == Node.NodeType.Block) {
			renderer.material.color = blockC;
		} else if (n.type == Node.NodeType.Start) {
			renderer.material.color = startC;
		} else if (n.type == Node.NodeType.End) {
			renderer.material.color = endC;
		}
	}

	void ResetAllColors () {
		var nodes = Grid.nodes;
		foreach (Node t in nodes) {
			ColorByType(t);
		}
	}

	List<Node> GetNeighbours (Node node, int iterations, List<Node> n) {
		if (iterations == 0) {
			return n;
		}
		foreach(Node t in node.neighbours) {
			n.Add(t);
			n = GetNeighbours(t, iterations - 1, n);
		}
		return n;
	}

	List<Node> DeleteDuplicates (List<Node> input) {
		var output = new List<Node>();
		foreach (Node t in input) {
			if (!output.Contains(t)) {
				output.Add(t);
			}
		}
		return output;
	}
}
