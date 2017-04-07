using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUtilities : MonoBehaviour {

	public LayerMask layerMask;
	static LayerMask mask;

	static AStar aStar;
	static public List<EnemyController> enemies { get; private set; }

	void Start () {
		aStar = FindObjectOfType<AStar>();
		mask = layerMask;
		enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
	}

	public static bool HasLineOfSight (Vector3 from, Transform to) {
		var dir = to.position - from;
		Ray ray = new Ray(from, dir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
			var colls = new List<Collider>(to.GetComponentsInChildren<Collider>());
			return colls.Contains(hit.collider);
		}
		return false;
	}

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

	public static Vector3 GetClosestLOSPoint (EnemyController calledFrom, Vector3 from, Transform to, bool withLOS) {

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

			bool validNode = true;
			foreach (EnemyController e in enemies) {
				if (e == calledFrom) {
					continue;
				}
				if (v == e.GetAtNode() || v == e.GetMovingToNode()) {
					validNode = false;
					break;
				}
			}
			if (!validNode) {
				continue;
			}

			if (v.type == Node.NodeType.Block) {
				continue;
			}

			if (HasLineOfSight(v.position, to) == withLOS) {
				Debug.DrawLine(v.position, to.position, Color.green);
				return v.position;
			} else {
				Debug.DrawLine(v.position, to.position, Color.red);
			}

			foreach (Node t in v.neighbours) {
				if (!discovered.Contains(t) && v.type != Node.NodeType.Block) {
					discovered.Add(t);
					q.Enqueue(t);
				}
			}
		}
		return Vector3.down;
	}

	public static void EnemyDied (EnemyController enemy) {
		enemies.Remove(enemy);
	}
}
