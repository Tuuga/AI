using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worldmap : MonoBehaviour {

	public LayerMask mask;
	public Vector2 scanRange;
	public Vector2 offset;
	public float density;

	List<Vector3> coverPoints = new List<Vector3>();
		
	void Start () {
		coverPoints = FindCoverPoints();

		// Debug
		foreach (Vector3 v in coverPoints) {
			Debug.DrawLine(v, v + Vector3.up * 5, Color.green, Mathf.Infinity);
		}
	}

	public List<Vector3> GetCoverPoints() {
		return coverPoints;
	}

	List<Vector3> FindCoverPoints () {
		var foundPoints = new List<Vector3>();
		for (int x = 0; x <= scanRange.x * density; x++) {
			for (int z = 0; z <= scanRange.y * density; z++) {
				var pos = new Vector3((x / density) + offset.x, 0, (z / density) + offset.y);
				var origin = pos + Vector3.up * 5;
				Ray ray = new Ray(origin, Vector3.down);
				var wall = Physics.Raycast(ray, Mathf.Infinity, mask);
				if (!wall) {
					foundPoints.Add(pos);
				}
			}
		}
		return foundPoints;
	}
}
