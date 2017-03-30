using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public float confidenceRegen;

	public float confidence { get; private set; }

	PathMovement movement;
	Transform player;

	void Start () {
		movement = GetComponent<PathMovement>();
		player = FindObjectOfType<PlayerController>().transform;
	}

	void Update () {

		if (!AIUtilities.HasLineOfSight(transform.position, player)) {
			confidence += confidenceRegen * Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			AffectConfidence(100f);
			movement.StopMoving();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			AffectConfidence(-100f);
			movement.StopMoving();
		}

		Vector3 moveToPoint = Vector3.down;
		if (confidence < 50f) {
			if (AIUtilities.HasLineOfSight(transform.position, player)) {
				moveToPoint = AIUtilities.GetPointInCover(transform.position, player);
			}
		} else {
			if (!AIUtilities.HasLineOfSight(transform.position, player)) {
				moveToPoint = AIUtilities.GetClosestPointWithLOS(transform.position, player);
			}
		}

		if (moveToPoint != Vector3.down) {
			movement.MoveToPoint(moveToPoint);
		}
	}

	public void AffectConfidence (float effect) {
		confidence = Mathf.Clamp(confidence + effect, 0, 100);
	}
}
