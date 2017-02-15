using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float confidenceRegen;

	public float confidence { get; private set; }

	EnemyCoverSystem coverSystem;
	EnemyAttacking attacking;
	EnemyMovement movement;
	Transform player;

	void Start () {
		coverSystem = GetComponent<EnemyCoverSystem>();
		attacking = GetComponent<EnemyAttacking>();
		movement = GetComponent<EnemyMovement>();
		player = FindObjectOfType<PlayerController>().transform;
	}

	void Update () {

		if (!coverSystem.HasLineOfSight(transform.position, player)) {
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

		if (confidence < 50f) {
			coverSystem.TakeCover();
		} else {
			attacking.MoveTowardsPlayer();
		}
	}

	public void AffectConfidence (float effect) {
		confidence = Mathf.Clamp(confidence + effect, 0, 100);
	}
}
