using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {

	public GameObject bulletPrefab;
	public float rpm;
	public float speed;
	public float coneOfFire;
	//public float damage;

	float lastShot;

	public void Shoot (Vector3 dir) {
		if (lastShot < Time.time - (60f / rpm)) {
			GameObject bulletIns = (GameObject)Instantiate(bulletPrefab, transform.position + dir + (Vector3.up * 0.5f), Quaternion.identity);
			var rb = bulletIns.GetComponent<Rigidbody>();

			var acc = Random.Range(-coneOfFire / 2, coneOfFire / 2);
			var finalDir = Quaternion.Euler(0, acc, 0) * dir;

			rb.AddForce(finalDir * speed, ForceMode.Impulse);
			lastShot = Time.time;
		}
	}
}
