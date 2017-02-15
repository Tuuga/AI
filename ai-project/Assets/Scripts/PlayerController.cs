using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Shooting shooting;
	PlayerAiming aiming;

	void Start () {
		shooting = GetComponent<Shooting>();
		aiming = GetComponent<PlayerAiming>();
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Mouse0)) {
			shooting.Shoot(aiming.dir);
		}
	}
}
