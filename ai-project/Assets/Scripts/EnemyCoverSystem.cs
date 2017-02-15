using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCoverSystem : MonoBehaviour {

	PlayerController player;
	List<Vector3> coverPoints = new List<Vector3>();
	EnemyMovement movement;

	void Start () {
		player = FindObjectOfType<PlayerController>();
		movement = GetComponent<EnemyMovement>();
	}

	void Update () {
		if (coverPoints.Count <= 0) { coverPoints = FindObjectOfType<Worldmap>().GetCoverPoints(); }
	}

	public void TakeCover () {
		if (HasLineOfSight(transform.position, player.transform)) {
			var noLOSPoints = PointsWithoutLOS(coverPoints); /*DEBUG ->*/ //print(noLOSPoints.Count);
			var closestPoint = GetClosestPoint(transform.position, noLOSPoints);

			if (movement.GetCornersToPoint(closestPoint).Length > 0 && movement.PathLenght(closestPoint) > 1f) {
				movement.MoveToPoint(closestPoint);
			}
		}
	}

	public bool HasLineOfSight (Vector3 from, Transform to) {
		var dir = to.position - from;
		Ray ray = new Ray(from, dir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return hit.transform.parent == to;
		}
		return false;
	}

	List<Vector3> PointsWithoutLOS (List<Vector3> l) {
		var noLOS = new List<Vector3>();
		var threats = GameObject.FindGameObjectsWithTag("Threat");
		foreach (Vector3 v in l) {
			if (Vector3.Distance(player.transform.position, v) < 1f) {
				continue;
			}

			bool potential = true;

			Vector3 cornerToCheck = new Vector3();
			for (int i = 0; i < threats.Length; i++) {
				if (!potential) { break; }
				cornerToCheck = new Vector3(0.5f, 0, 0.5f);
				for (int j = 0; j < 4; j++) {
					cornerToCheck = Quaternion.Euler(0, 90 * j, 0) * cornerToCheck;
					if (HasLineOfSight(v + cornerToCheck, threats[i].transform)) {
						potential = false;
						break;
					}
				}
			}

			if (!potential) { continue; }
			cornerToCheck = new Vector3(0.5f, 0, 0.5f);
			for (int i = 0; i < 4; i++) {
				cornerToCheck = Quaternion.Euler(0, 90 * i, 0) * cornerToCheck;
				if (HasLineOfSight(v + cornerToCheck, player.transform)) {
					potential = false;
					break;
				}
			}
			
			if (potential) {
				noLOS.Add(v);
			}
		}
		return noLOS;
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
