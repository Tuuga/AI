using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfidenceBar : MonoBehaviour {

	EnemyController enemy;
	Slider slider;

	void Start () {
		enemy = FindObjectOfType<EnemyController>();
		slider = GetComponent<Slider>();
	}

	void Update () {
		slider.value = enemy.confidence;
	}
}
