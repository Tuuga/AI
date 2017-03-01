using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovement : MonoBehaviour {

	public float speed;
	public bool recalculate;

	UserInterface ui;
	AStar aStar;

	Node endNode = new Node();
	Node startNode;

	public List<Node> path { get; private set; }

	Coroutine currentRunning;

	void Start () {
		ui = FindObjectOfType<UserInterface>();
		aStar = FindObjectOfType<AStar>();
	}

	void Update () {
		// Start and end exists
		// the endNode is not the same as uis endNode

		if (ui.end != null && endNode != ui.end || recalculate) {
			endNode = ui.end;
			recalculate = false;

			startNode = Grid.GetNodeWorldPoint(transform.position);
			path = aStar.Search(startNode, ui.end);

			VisualizePath();

			if (currentRunning != null) {
				StopCoroutine(currentRunning);
			}
			currentRunning = StartCoroutine(Move());
		}
	}

	IEnumerator Move () {
		while (path.Count > 0) {
			var dir = (path[0].position - transform.position).normalized;
			var dist = Vector3.Distance(transform.position, path[0].position);

			while (dist > 0) {
				transform.position += dir * speed * Time.deltaTime;
				dist -= speed * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			path.RemoveAt(0);
		}
	}

	float TimeToGoal () {
		float time = 0;
		Vector3 lastPos = transform.position;
		foreach(Node n in path) {
			var dist = Vector3.Distance(lastPos, n.position);
			time += dist / speed;
			lastPos = n.position;
		}
		return time;
	}

	void VisualizePath () {
		var time = TimeToGoal();
		for (int i = 0; i < path.Count - 1; i++) {
			Debug.DrawLine(path[i].position, path[i + 1].position, Color.green, time);
		}
	}
}
