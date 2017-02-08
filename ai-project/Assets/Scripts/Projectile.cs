using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	void Start () {
		Destroy(gameObject, 5f);
	}

	void OnCollisionEnter (Collision c) {
		Destroy(gameObject);
	}
}
