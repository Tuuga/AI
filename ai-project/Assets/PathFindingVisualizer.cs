using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingVisualizer : MonoBehaviour {

	public Color emptyColor, blockColor, startColor, endColor, pathColor, visitedColor;
	public float visualizationSpeed;

	bool visualizerRunning;

	Coroutine currentVis;

	public void VisualizePath (List<Node> path) {
		if (currentVis != null) { StopCoroutine(currentVis); }
		currentVis = StartCoroutine(Visualize(path, pathColor));
	}

	public void VisualizeVisited (List<Node> visited) {
		if (currentVis != null) { StopCoroutine(currentVis); }
		currentVis = StartCoroutine(Visualize(visited, visitedColor));
	}

	IEnumerator Visualize (List<Node> p, Color c) {
		visualizerRunning = true;
		while (p.Count > 0) {
			if (p[0].type == Node.NodeType.Empty) {
				ColorNode(p[0], c);
			}
			p.RemoveAt(0);

			if (visualizationSpeed >= 0) {
				yield return new WaitForSeconds(visualizationSpeed);
			}
		}
		visualizerRunning = false;
		currentVis = null;
	}

	public void ColorNodeByType (Node n) {
		if (n.type == Node.NodeType.Empty) {
			ColorNode(n, emptyColor);
		} else if (n.type == Node.NodeType.Block) {
			ColorNode(n, blockColor);
		}
	}

	public void ColorStartNode (Node n) {
		ColorNode(n, startColor);
	}

	public void ColorEndNode (Node n) {
		ColorNode(n, endColor);
	}

	void ColorNode (Node n, Color c) {
		var renderer = n.visual.GetComponent<MeshRenderer>();
		renderer.material.color = c;
	}

	public void ResetAllColors (Node start, Node end) {
		var nodes = Grid.nodes;
		if (currentVis != null) { StopCoroutine(currentVis); }

		if (start != null) { ColorStartNode(start); }
		if (end != null) { ColorEndNode(end); }

		foreach (Node n in nodes) {
			if (n == start || n == end) {
				continue;
			}
			ColorNodeByType(n);
		}
	}
}