using UnityEngine;
using System.Collections;

public class TestGridGeneratorAlgorithm : GridGeneratorAlgorithm {

	public int width;
	public int height;

	public override int generationRegionWidth { get { return width; } }
	public override int generationRegionHeight { get { return height; } }

	bool done = true;

	void Update() {
		if (done && Application.isPlaying) {
			done = false;
			Grid grid = GetComponent<Grid> ();
			grid.LoadRegion (0, 0);
		}
	}

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile {
					id = 1,
					subId = -1,
					shape = TileShape.FULL
				};
			}
		}

		return tiles;
	}
}

