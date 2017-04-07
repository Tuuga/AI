using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUtilities : MonoBehaviour {

	public LayerMask layerMask;
	static LayerMask mask;

	static AStar aStar;
	static public List<EnemyController> enemies { get; private set; }

	static PlayerController player;

	void Start () {
		aStar = FindObjectOfType<AStar>();
		mask = layerMask;
		enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
		player = FindObjectOfType<PlayerController>();
	}

	public static bool HasLineOfSight (Vector3 from, Vector3 to, MonoBehaviour target = null) {
		var dir = to - from;
		Ray ray = new Ray(from, dir);
		RaycastHit hit;

		bool rayCast = Physics.Raycast(ray, out hit, dir.magnitude, mask);
		if (rayCast) {
			if (target != null) {
				var colls = new List<Collider>(target.GetComponentsInChildren<Collider>());
				return colls.Contains(hit.collider);
			}
		}
		return rayCast;
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

	public static Vector3 GetClosestLOSPoint (EnemyController calledFrom, Vector3 from, Vector3 to, bool withLOS, MonoBehaviour target = null) {

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

			if (v.type == Node.NodeType.Block) {
				continue;
			}
			if (v == Grid.GetNodeWorldPoint(player.transform.position)) {
				continue;
			}

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

			if (HasLineOfSight(v.position, to, target) == withLOS) {
				Debug.DrawLine(v.position, to, Color.green);
				return v.position;
			} else {
				Debug.DrawLine(v.position, to, Color.red);
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
