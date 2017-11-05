using UnityEngine;
using System.Collections;

using Gridlike;

public class PerlinNoiseGeneratorAlgorithm : GridGeneratorAlgorithm {

	[Range(0, 1)]
	public float threshold = 0.5f;

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				float noise = Mathf.PerlinNoise ((x + i)/50f, (y + j)/50f);

				tiles [i, j] = new Tile {
					id = noise > threshold ? 1 : 0
				};
			}
		}

		return tiles;
	}
}

