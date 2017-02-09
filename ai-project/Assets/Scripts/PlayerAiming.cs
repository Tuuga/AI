using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour {

	public Vector3 dir { get; private set; }

	void Update () {
		var mousePos = GetWorldMousePosition();
		dir = (mousePos - transform.position).normalized;
		var rot = Quaternion.LookRotation(dir, Vector3.up);
		transform.rotation = rot;
	}

	Vector3 GetWorldMousePosition () {
		Plane plane = new Plane(Vector3.up, Vector3.up * transform.position.y);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist;
		plane.Raycast(ray, out dist);
		return ray.GetPoint(dist);
	}
}
