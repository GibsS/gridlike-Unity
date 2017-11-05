using UnityEngine;
using System.Collections;

using Gridlike;

public class TestGridGeneratorAlgorithm : GridGeneratorAlgorithm {

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile {
					id = (i % Grid.REGION_SIZE) != 0 ? 1 : 0
				};
			}
		}

		return tiles;
	}
}

