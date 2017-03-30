using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovement : MonoBehaviour {

	public float speed;
	AStar aStar;

	Node endNode = new Node();
	Node startNode;

	public List<Node> path { get; private set; }

	Coroutine currentRunning;

	void Start () {
		aStar = FindObjectOfType<AStar>();
	}

	public void MoveToPoint (Vector3 point) {
		if (endNode == Grid.GetNodeWorldPoint(point)) {
			return;
		}

		startNode = Grid.GetNodeWorldPoint(transform.position);
		endNode = Grid.GetNodeWorldPoint(point);
		path = aStar.Search(startNode, endNode);

		if (currentRunning != null) { StopCoroutine(currentRunning); }
		currentRunning = StartCoroutine(Move());
		VisualizePath();
	}

	public void StopMoving () {
		StopCoroutine(currentRunning);
		currentRunning = null;
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
