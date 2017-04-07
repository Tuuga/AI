using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public GameObject lastKnownVisual;

	public float confidenceRegen;

	public float confidence { get; private set; }

	PathMovement movement;
	EnemyAiming aiming;
	EnemyShooting shooting;

	PlayerController player;
	Vector3 lastKnownPlayerLocation;

	Node movingToNode;

	void Start () {
		movement = GetComponent<PathMovement>();
		aiming = GetComponent<EnemyAiming>();
		shooting = GetComponent<EnemyShooting>();

		player = FindObjectOfType<PlayerController>();
	}

	void Update () {
		bool hasLOSToPlayer = AIUtilities.HasLineOfSight(transform.position, player.transform.position, player);

		if (hasLOSToPlayer) {
			lastKnownPlayerLocation = player.transform.position;
			lastKnownVisual.transform.position = lastKnownPlayerLocation;
		}

		var aimPos = lastKnownPlayerLocation;
		aimPos.y = 0;
		aiming.LookAt(aimPos);

		if (!hasLOSToPlayer) {
			confidence += confidenceRegen * Time.deltaTime;
		}

		// DEBUG INPUTS
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			AffectConfidence(100f);
			movement.StopMoving();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			AffectConfidence(-100f);
			movement.StopMoving();
		}

		Vector3 moveToPoint = Vector3.down;

		// Getting in cover
		if (confidence < 50f) {
			if (hasLOSToPlayer /*&& lastPlayerPos != player.transform.position*/) {
				moveToPoint = AIUtilities.GetClosestLOSPoint(this, transform.position, player.transform.position, false, player);
			}

			// Attacking player
		} else {
			if (hasLOSToPlayer) {
				shooting.Shoot(transform.forward);
			} else {
				if (AIUtilities.HasLineOfSight(transform.position, lastKnownPlayerLocation)) {
					print("has los");
					moveToPoint = lastKnownPlayerLocation;
				} else {
					print("no los");
					moveToPoint = AIUtilities.GetClosestLOSPoint(this, transform.position, lastKnownPlayerLocation, true);
				}
			}
		}

		if (moveToPoint != Vector3.down) {
			movement.MoveToPoint(moveToPoint);
			movingToNode = Grid.GetNodeWorldPoint(moveToPoint);
		}
	}

	public void AffectConfidence (float effect) {
		confidence = Mathf.Clamp(confidence + effect, 0, 100);
	}

	public void Die () {
		AIUtilities.EnemyDied(this);
	}

	public Node GetAtNode () {
		return Grid.GetNodeWorldPoint(transform.position);
	}

	public Node GetMovingToNode () {
		return movingToNode;
	}
}
