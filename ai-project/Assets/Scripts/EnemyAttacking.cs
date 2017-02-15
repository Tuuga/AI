using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacking : MonoBehaviour {

	List<Vector3> coverPoints = new List<Vector3>();
	Transform player;
	EnemyMovement movement;

	void Start () {
		player = FindObjectOfType<PlayerController>().transform;
		movement = GetComponent<EnemyMovement>();
	}

	void Update () {
		if (coverPoints.Count <= 0) { coverPoints = FindObjectOfType<Worldmap>().GetCoverPoints(); }
	}

	public void MoveTowardsPlayer () {
		var dist = Vector3.Distance(transform.position, player.position);
		var losPoints = PointsWithLOS(coverPoints);
		var closestPoint = GetClosestPoint(transform.position, losPoints);
		movement.MoveToPoint(closestPoint);
	}

	bool HasLineOfSight (Vector3 from, Transform to) {
		var dir = to.position - from;
		Ray ray = new Ray(from, dir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return hit.transform.parent == to;
		}
		return false;
	}

	List<Vector3> PointsWithLOS (List<Vector3> l) {
		var los = new List<Vector3>();
		foreach (Vector3 v in l) {
			if (HasLineOfSight(v, player)) {
				los.Add(v);
			}
		}
		return los;
	}

	Vector3 GetClosestPoint (Vector3 from, List<Vector3> pL) {
		var closestPoint = from;
		float smallestDist = Mathf.Infinity;
		foreach (Vector3 v in pL) {
			var dist = movement.PathLenght(v);

			if (dist < smallestDist) {
				smallestDist = dist;
				closestPoint = v;
			}
		}
		return closestPoint;
	}
}
