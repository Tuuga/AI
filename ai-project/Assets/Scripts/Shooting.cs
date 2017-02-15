using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {

	public GameObject bulletPrefab;
	public float rpm;
	public float speed;
	public float coneOfFire;
	public float intimidation;
	public float intimidationAngle;
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
			Intimidate(dir);
		}
	}

	void Intimidate (Vector3 dir) {
		var enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
		var enemiesInLOS = GetEnemiesInLOS(enemies);
		var enemiesInLOSAndAngle = GetEnemiesInAngle(enemiesInLOS, intimidationAngle, dir);

		foreach (EnemyController e in enemiesInLOSAndAngle) {
			e.AffectConfidence(intimidation);
		}
	}

	List<EnemyController> GetEnemiesInAngle (List<EnemyController> enemies, float angle, Vector3 dir) {
		List<EnemyController> enemiesInAngle = new List<EnemyController>();

		foreach (EnemyController e in enemies) {
			var angleFromPlayer = Vector3.Angle(dir, e.transform.position - transform.position);
			if (angleFromPlayer < angle) {
				enemiesInAngle.Add(e);
			}
		}
		return enemiesInAngle;
	}
	
	List<EnemyController> GetEnemiesInLOS (List<EnemyController> enemies) {
		List<EnemyController> enemiesInLOS = new List<EnemyController>();

		foreach (EnemyController e in enemies) {
			var dir = e.transform.position - transform.position;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, dir, out hit)) {
				if (hit.transform.parent == e.transform) {
					enemiesInLOS.Add(e);
				}
			}
		}
		return enemiesInLOS;
	}
}
