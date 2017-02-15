using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	public float movementSpeed;

	bool moving;
	NavMeshAgent agent;
	Coroutine currentlyRunning;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}

	public void MoveToPoint (Vector3 point) {
		if (!moving && Vector3.Distance(transform.position, point) > 0.2f) {
			currentlyRunning = StartCoroutine(MoveTo(GetCornersToPoint(point)));
		}
	}

	public void StopMoving () {
		if (moving) {
			StopCoroutine(currentlyRunning);
			moving = false;
		}		
	}

	IEnumerator MoveTo (Vector3[] corners) {
		moving = true;

		// DEBUG
		for (int i = 0; i < corners.Length - 1; i++) {
			Debug.DrawLine(corners[i], corners[i + 1], Color.cyan, 2f);
		}

		for (int i = 1; i < corners.Length; i++) {
			var dist = Vector3.Distance(transform.position, corners[i]);
			var dir = (corners[i] - transform.position).normalized;
			while (dist > 0) {
				transform.position += dir * movementSpeed * Time.deltaTime;
				dist -= movementSpeed * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
		moving = false;
	}

	public float PathLenght (Vector3 endPoint) {
		var corners = GetCornersToPoint(endPoint);
		float dist = 0f;
		for (int i = 0; i < corners.Length - 1; i++) {
			dist += Vector3.Distance(corners[i], corners[i + 1]);
		}
		return dist;
	}

	public Vector3[] GetCornersToPoint (Vector3 point) {
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(point, path);
		return path.corners;
	}
}
