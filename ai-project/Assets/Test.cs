using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public Vector2 node;
	public Node selected;
	Grid grid;

	void Start () {
		grid = FindObjectOfType<Grid>();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.F)) {
			var index = Grid.GetNodeIndex((int)node.x, (int)node.y);
			selected = Grid.nodes[index];
			print(index);
		}
	}
}
