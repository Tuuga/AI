using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAiming : MonoBehaviour {

	public float turningSpeed;

	public void LookAt (Vector3 point) {
		var dir = (point - transform.position).normalized;
		var rot = Quaternion.LookRotation(dir, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turningSpeed * Time.deltaTime);
	}
}
