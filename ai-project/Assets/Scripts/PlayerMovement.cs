using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed;

	void Update () {
		var dir = (Vector3.right * Input.GetAxis("Horizontal")) + (Vector3.forward * Input.GetAxis("Vertical"));
		dir = dir.magnitude > 1 ? dir.normalized : dir;
		transform.position += dir * speed * Time.deltaTime;
	}
}
