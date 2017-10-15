using UnityEngine;
using System.Collections;

public class TestGridGeneratorAlgorithm : GridGeneratorAlgorithm {

	// TODO make sure to keep it big enough
	public int width;
	public int height;

	// TODO move this to grid generator
	public override int generationRegionWidth { get { return width; } }
	public override int generationRegionHeight { get { return height; } }

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile {
					id = (j + y > i + x) ? 0 : 1,
					subId = -1
				};
			}
		}

		return tiles;
	}
}

