using UnityEngine;
using System.Collections;

public class TestGridGeneratorAlgorithm : GridGeneratorAlgorithm {

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile {
					id = 1, //(j + y > i + x) ? 0 : 1,
					subId = -1
				};
			}
		}

		return tiles;
	}
}

