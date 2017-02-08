using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

	EnemyCoverSystem coverSystem;
	//NavMeshAgent agent;

	void Start () {
		coverSystem = GetComponent<EnemyCoverSystem>();
		//agent = GetComponent<NavMeshAgent>();
	}

	void Update () {
		coverSystem.TakeCover();
	}
}
