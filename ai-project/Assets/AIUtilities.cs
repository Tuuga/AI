using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUtilities : MonoBehaviour {

	static AStar aStar;

	void Start () {
		aStar = FindObjectOfType<AStar>();
	}

	public static bool HasLineOfSight (Vector3 from, Transform to) {
		var dir = to.position - from;
		Ray ray = new Ray(from, dir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return hit.transform.parent == to;
		}
		return false;
	}

	//public static List<Vector3> PointsWithLOS (List<Vector3> points, Transform target) {
	//	var los = new List<Vector3>();
	//	foreach (Vector3 v in points) {
	//		if (HasLineOfSight(v, target)) {
	//			los.Add(v);
	//		}
	//	}
	//	return los;
	//}

	//public static List<Vector3> PointsWithoutLOS (List<Vector3> points, Transform target) {
	//	var los = new List<Vector3>();
	//	foreach (Vector3 v in points) {
	//		if (!HasLineOfSight(v, target)) {
	//			los.Add(v);
	//		}
	//	}
	//	return los;
	//}

	public static List<Vector3> NodesToPoints (List<Node> nodes) {
		var points = new List<Vector3>();
		foreach (Node n in nodes) {
			points.Add(n.position);
		}
		return points;
	}

	public static Vector3 GetClosestPoint (Vector3 from, List<Vector3> pL) {
		var closestPoint = from;
		float smallestDist = Mathf.Infinity;
		foreach (Vector3 v in pL) {
			var dist = PathLength(from, v);

			if (dist < smallestDist) {
				smallestDist = dist;
				closestPoint = v;
			}
		}
		return closestPoint;
	}

	public static float PathLength (Vector3 startPoint, Vector3 endPoint) {
		var start = Grid.GetNodeWorldPoint(startPoint);
		var end = Grid.GetNodeWorldPoint(endPoint);
		var path = NodesToPoints(aStar.Search(start, end));

		float dist = 0f;
		for (int i = 0; i < path.Count - 1; i++) {
			dist += Vector3.Distance(path[i], path[i + 1]);
		}
		return dist;
	}

	public static Vector3 GetClosestPointWithLOS (Vector3 from, Transform to) {

		Node start = Grid.GetNodeWorldPoint(from);
		if (start == null) {
			return Vector3.down;
		}

		var q = new Queue<Node>();
		q.Enqueue(start);

		var discovered = new List<Node>();
		discovered.Add(start);

		while (q.Count > 0) {
			var v = q.Dequeue();

			if (HasLineOfSight(v.position, to)) {
				return v.position;
			}

			foreach (Node t in v.neighbours) {
				if (!discovered.Contains(t) && t.type != Node.NodeType.Block) {
					q.Enqueue(t);
					discovered.Add(v);
				}
			}
		}
		return Vector3.down;
	}

	public static Vector3 GetPointInCover (Vector3 from, Transform to) {
		Node start = Grid.GetNodeWorldPoint(from);
		if (start == null) {
			return Vector3.down;
		}

		var q = new Queue<Node>();
		q.Enqueue(start);

		var discovered = new List<Node>();
		discovered.Add(start);

		while (q.Count > 0) {
			var v = q.Dequeue();

			if (!HasLineOfSight(v.position, to)) {
				return v.position;
			}

			foreach (Node t in v.neighbours) {
				if (!discovered.Contains(t) && t.type != Node.NodeType.Block) {
					q.Enqueue(t);
					discovered.Add(v);
				}
			}
		}
		return Vector3.down;
	}
}
