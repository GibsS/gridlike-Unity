using UnityEngine;
using System.Collections;

using Gridlike;

namespace Gridship {

	/// <summary>
	/// A small example of a generation algorithm.
	/// </summary>
	public class Generation1 : GridGeneratorAlgorithm {

		[Range(0, 1)]
		public float groundThreshold = 0.5f;
		public float groundGranularity = 30;
		public int groundTileId = 1;

		[Range(0, 1)]
		public float mineralThreshold = 0.5f;
		public float mineralGranularity = 30;
		public int mineralTileId = 1;

		public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
			if (groundTileId == 0) groundTileId = 1;
			
			Tile[,] tiles = new Tile[width, height];

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					// ground
					float noise = Mathf.PerlinNoise ((x + i)/groundGranularity, (y + j)/groundGranularity);

					tiles [i, j] = new Tile {
						id = noise > groundThreshold ? groundTileId : 0
					};

					// mineral
					noise = Mathf.PerlinNoise((x + i)/mineralGranularity, (y + j)/mineralGranularity);

					if (noise > mineralThreshold && tiles[i, j].id == groundTileId) tiles [i, j].id = mineralTileId;
				}
			}

			return tiles;
		}
	}

}