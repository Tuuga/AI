﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class UserInterface : MonoBehaviour {

	enum PaintMode { Empty, Block, Start, End }
	PaintMode currentPaintMode;

	public float visualizationSpeed;
	public Color emptyC, blockC, startC, endC, processedC, pathC;

	public Text processedCount, pathCount, methodType, time, paintMode;

	[TextArea(15, 100)]
	public string keys;

	public enum SearchMethod { BFS, DFS, AStar }
	public SearchMethod useMethod;

	BFS bfs;
	DFS dfs;
	AStar astar;

	Node start;
	Node end;

	List<Node> processed;
	List<Node> path;

	bool visualizeRunning;
	Coroutine currentVis;

	void Start () {
		bfs = FindObjectOfType<BFS>();
		dfs = FindObjectOfType<DFS>();
		astar = FindObjectOfType<AStar>();
	}

	void Update () {
		if (Input.GetKey(KeyCode.Mouse0)) {
			var foundNode = MouseSelect();
			if (foundNode != null) {
				if (Input.GetKeyDown(KeyCode.Mouse0)) {			// If pressed down this frame
					if (currentPaintMode == PaintMode.Start) {
						foundNode.SetNodeType(Node.NodeType.Empty);
						if (start != null) { ChangeColor(start, emptyC); }
						start = foundNode;
					} else if (currentPaintMode == PaintMode.End) {
						foundNode.SetNodeType(Node.NodeType.Empty);
						if (end != null) { ChangeColor(end, emptyC); }
						end = foundNode;
					}
				} else {
					if (currentPaintMode == PaintMode.Empty) {
						foundNode.SetNodeType(Node.NodeType.Empty);
						
					} else if (currentPaintMode == PaintMode.Block) {
						foundNode.SetNodeType(Node.NodeType.Block);
					}
				}
				ColorByType(foundNode);
			}
		}

		// Finalize grid and search
		if (Input.GetKeyDown(KeyCode.Space)) {
			ResetAllColors();

			bool canSearch = true;
			if (start == null) {
				UnityEngine.Debug.LogError("NO START NODE");
				canSearch = false;
			}
			if (end == null) {
				UnityEngine.Debug.LogError("NO END NODE");
				canSearch = false;
			}
			if (!canSearch) { return; }

			Stopwatch sw = new Stopwatch();
			sw.Start();
			if (useMethod == SearchMethod.BFS) {
				path = bfs.Search(start, end);
				processed = bfs.processed;
			} else if (useMethod == SearchMethod.DFS) {
				path = dfs.Search(start, end);
				processed = dfs.processed;
			} else if (useMethod == SearchMethod.AStar) {
				path = astar.Search(start, end);
				processed = astar.processed;
			}

			sw.Stop();

			processedCount.text = "Processed: " + processed.Count;
			pathCount.text = "Path: " + path.Count;
			methodType.text = "Method: " + useMethod.ToString();
			time.text = "Time: " + sw.Elapsed;
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

		// PaintMode
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			currentPaintMode = PaintMode.Empty;
			paintMode.text = "Paint Mode: " + currentPaintMode.ToString() + " <color=white>[ ]</color>";
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			currentPaintMode = PaintMode.Block;
			paintMode.text = "Paint Mode: " + currentPaintMode.ToString() + " <color=black>[ ]</color>";
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			currentPaintMode = PaintMode.Start;
			paintMode.text = "Paint Mode: " + currentPaintMode.ToString() + " <color=cyan>[ ]</color>";
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			currentPaintMode = PaintMode.End;
			paintMode.text = "Paint Mode: " + currentPaintMode.ToString() + " <color=red>[ ]</color>";
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
			if (p[0].type == Node.NodeType.Empty && !(p[0] == start || p[0] == end)) {
				ChangeColor(p[0], c);
			}
			p.RemoveAt(0);

			if (visualizationSpeed >= 0) {
				yield return new WaitForSeconds(visualizationSpeed);
			}
		}
		visualizeRunning = false;
		currentVis = null;
		yield return null;
	}

	void ChangeColor (Node n, Color c) {
		var renderer = n.visual.GetComponent<MeshRenderer>();
		renderer.material.color = c;
	}

	void ColorByType (Node n) {
		var renderer = n.visual.GetComponent<MeshRenderer>();
		if (n != start && n != end) {
			if (n.type == Node.NodeType.Empty) {
				renderer.material.color = emptyC;
			} else if (n.type == Node.NodeType.Block) {
				renderer.material.color = blockC;
			}
		} else {
			if (n == start) {
				renderer.material.color = startC;
			} else if (n == end) {
				renderer.material.color = endC;
			}
		}
	}

	void ResetAllColors () {
		var nodes = Grid.nodes;
		foreach (Node t in nodes) {
			ColorByType(t);
		}
	}
}
