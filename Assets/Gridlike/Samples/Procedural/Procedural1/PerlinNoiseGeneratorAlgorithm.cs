using UnityEngine;
using System.Collections;

using Gridlike;

public class PerlinNoiseGeneratorAlgorithm : GridGeneratorAlgorithm {

	[Range(0, 1)]
	public float threshold = 0.5f;

	public float granularity = 30;

	public int tileId = 1;

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		if (tileId == 0) tileId = 1;
		
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				float noise = Mathf.PerlinNoise ((x + i)/granularity, (y + j)/granularity);

				tiles [i, j] = new Tile {
					id = noise > threshold ? tileId : 0
				};
			}
		}

		return tiles;
	}
}

