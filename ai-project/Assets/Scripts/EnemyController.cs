using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public float confidenceRegen;

	public float confidence { get; private set; }

	PathMovement movement;
	EnemyAiming aiming;
	EnemyShooting shooting;

	Transform player;
	Vector3 lastPlayerPos;

	Node movingToNode;

	void Start () {
		movement = GetComponent<PathMovement>();
		aiming = GetComponent<EnemyAiming>();
		shooting = GetComponent<EnemyShooting>();

		player = FindObjectOfType<PlayerController>().transform;
	}

	void Update () {

		var playerPos = player.position;
		playerPos.y = 0;
		aiming.LookAt(playerPos);

		if (!AIUtilities.HasLineOfSight(transform.position, player)) {
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
		if (confidence < 50f) {
			if (AIUtilities.HasLineOfSight(transform.position, player) && lastPlayerPos != player.position) {
				moveToPoint = AIUtilities.GetClosestLOSPoint(this, transform.position, player, false);
			}
		} else {
			if (!AIUtilities.HasLineOfSight(transform.position, player) && lastPlayerPos != player.position) {
				moveToPoint = AIUtilities.GetClosestLOSPoint(this, transform.position, player, true);
			} else if (AIUtilities.HasLineOfSight(transform.position, player)) {
				shooting.Shoot(transform.forward);
			}
		}

		if (moveToPoint != Vector3.down) {
			movement.MoveToPoint(moveToPoint);
			movingToNode = Grid.GetNodeWorldPoint(moveToPoint);
		}

		lastPlayerPos = player.position;
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
