using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapCreation : EditorWindow {

	[MenuItem("Window/Map Creation")]
	static void Init () {
		MapCreation window = (MapCreation)GetWindow(typeof(MapCreation));
		window.Show();
	}

	GameObject tilePrefab;
	Vector2 mapSize;

	void OnGUI () {
		EditorGUILayout.BeginHorizontal();
		mapSize = EditorGUILayout.Vector2Field("Map Size", mapSize);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Create Map")) {
			var map = FindObjectOfType<Map>();

			// Destroys previous tiles
			var allTiles = FindObjectsOfType<TileProperties>();
			foreach (TileProperties t in allTiles) {
				DestroyImmediate(t.gameObject, false);
			}

			// Spawns tiles
			int index = 0;
			for (int y = 0; y < mapSize.y; y++) {
				for (int x = 0; x < mapSize.x; x++) {
					var tileIns = (GameObject)Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
					tileIns.transform.parent = GameObject.Find("Map").transform;
					
					var tile = tileIns.GetComponent<TileProperties>();
					tile.SetCoordinates(x, y);
					tile.index = index;
					index++;
				}
			}

			Camera.main.transform.position = new Vector3((mapSize.x - 1) / 2, Mathf.Max(mapSize.x, mapSize.y), (mapSize.y - 1) / 2);
			map.SetMapSize(mapSize);
		}
	}
}
