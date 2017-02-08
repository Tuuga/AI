using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCoverSystem : MonoBehaviour {

	public float movSpeed;

	PlayerMovement player;
	List<Vector3> coverPoints = new List<Vector3>();
	NavMeshAgent agent;
	public bool movingToCover { get; private set; }

	void Start () {
		player = FindObjectOfType<PlayerMovement>();
		agent = GetComponent<NavMeshAgent>();
	}

	void Update () {
		if (coverPoints.Count <= 0) { coverPoints = FindObjectOfType<Worldmap>().GetCoverPoints(); }
	}

	public void TakeCover () {
		if (!movingToCover) {
			var noLOSPoints = PointsWithoutLOS(coverPoints);
			print(noLOSPoints.Count);
			var closestPoint = GetClosestPoint(transform.position, noLOSPoints);

			NavMeshPath path = new NavMeshPath();
			agent.CalculatePath(closestPoint, path);
			if (path.corners.Length > 0 && PathLenght(path.corners) > 1f) {
				StartCoroutine(MoveToCover(path.corners));
			}
		}
	}

	IEnumerator MoveToCover (Vector3[] corners) {
		movingToCover = true;

		for (int i = 0; i < corners.Length - 1; i++) {
			Debug.DrawLine(corners[i], corners[i + 1], Color.cyan, 2f);
		}

		for (int i = 1; i < corners.Length; i++) {
			var dist = Vector3.Distance(transform.position, corners[i]);
			var dir = (corners[i] - transform.position).normalized;
			while (dist > 0) {
				transform.position += dir * movSpeed * Time.deltaTime;
				dist -= movSpeed * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}	
		movingToCover = false;
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
		var closestPoint = Vector3.up * 1000;
		float smallestDist = Mathf.Infinity;
		foreach (Vector3 v in pL) {
			NavMeshPath path = new NavMeshPath();
			agent.CalculatePath(v, path);
			var dist = PathLenght(path.corners);

			if (dist < smallestDist) {
				smallestDist = dist;
				closestPoint = v;
			}
		}
		return closestPoint;
	}

	float PathLenght (Vector3[] corners) {
		float dist = 0f;
		for (int i = 0; i < corners.Length - 1; i++) {
			dist += Vector3.Distance(corners[i], corners[i + 1]);
		}
		return dist;
	}
}
