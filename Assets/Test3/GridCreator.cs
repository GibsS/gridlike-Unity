using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour {

	public TileAtlas atlas;

	Grid grid;

	void OnGUI() {
		if (GUI.Button (new Rect (10, 10, 200, 40), "Create empty grid (without collider)")) {
			Clear ();

			grid = Grid.CreateGrid (new Vector2 (10, 10), atlas, true, false); 
		}
		if (GUI.Button (new Rect (10, 50, 200, 40), "Create grid with tiles (without sprites)")) {
			Clear ();

			Tile[,] tiles = CreateTiles ();

			grid = Grid.CreateGrid (new Vector2 (-10, -10), atlas, tiles, false, true); 
		}

		/*
		if (GUI.Button (new Rect (10, 10, 200, 40), "Create empty grid")) {
			Clear ();

			grid = Grid.CreateSaveGrid (new Vector2 (10, 10), atlas, "test", false, true, false); 
		}
		if (GUI.Button (new Rect (10, 10, 200, 40), "Create empty grid (without collider)")) {
			Clear ();

			Tile[,] tiles = CreateTiles ();

			grid = Grid.CreateSaveGrid (new Vector2 (10, 10), atlas, tiles, "test", false, true, false); 
		}


		if (GUI.Button (new Rect (10, 10, 200, 40), "Create empty grid (without collider)")) {
			Clear ();

			grid = Grid.CreateProceduralGrid (new Vector2 (10, 10), atlas, "test", false, true, false); 
		}
		if (GUI.Button (new Rect (10, 10, 200, 40), "Create empty grid (without collider)")) {
			Clear ();

			Tile[,] tiles = CreateTiles ();

			grid = Grid.CreateProceduralGrid (new Vector2 (10, 10), atlas, tiles, "test", false, true, false); 
		}
		*/
	}

	void Clear() {
		if (grid != null) {
			DestroyImmediate (grid.gameObject);
		}
	}

	Tile[,] CreateTiles() {
		Tile[,] tiles = new Tile[60, 60];

		for (int i = 0; i < 60; i++) {
			for (int j = 0; j < 60; j++) {
				tiles [i, j] = new Tile {
					id = 1
				};
			}
		}

		return tiles;
	}
}