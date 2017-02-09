using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	EnemyCoverSystem coverSystem;
	EnemyAttacking attacking;
	EnemyMovement movement;

	// DEBUG
	bool toggle;

	void Start () {
		coverSystem = GetComponent<EnemyCoverSystem>();
		attacking = GetComponent<EnemyAttacking>();
		movement = GetComponent<EnemyMovement>();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {	
			toggle = !toggle;
			movement.StopMoving();
		}

		if (toggle) {
			coverSystem.TakeCover();
		} else {
			attacking.MoveTowardsPlayer();
		}
	}
}
