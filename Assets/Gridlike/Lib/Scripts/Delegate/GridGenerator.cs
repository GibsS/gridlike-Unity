using UnityEngine;
using System.Collections.Generic;

public class LargeRegion {

	public int regionX;
	public int regionY;
	public int regionWidth;
	public int regionHeight;

	public Tile[,] tiles;
}

public class GridGenerator : GridDataDelegate {

	List<LargeRegion> largeRegions;

	Tile[,] tempTiles;

	[SerializeField] GridGeneratorAlgorithm algorithm;

	void Start() {
		largeRegions = new List<LargeRegion> ();
		tempTiles = new Tile[Grid.REGION_SIZE, Grid.REGION_SIZE];
	}

	public override void ResetDelegate() {
		base.ResetDelegate ();

		if (algorithm == null) {
			algorithm = GetComponent<GridGeneratorAlgorithm> ();
		}
	}
	public void SetAlgorithm(GridGeneratorAlgorithm algorithm) {
		this.algorithm = algorithm;
	}

	public override Tile[,] LoadTiles (bool dataPresent, int regionX, int regionY) {
		LargeRegion region = GetRegions (regionX, regionY);

		if (region == null) {
			int largeRegionX = Mathf.FloorToInt (regionX / (float)algorithm.generationRegionWidth) * algorithm.generationRegionWidth;
			int largeRegionY = Mathf.FloorToInt (regionY / (float)algorithm.generationRegionHeight) * algorithm.generationRegionHeight;

			Debug.Log ("Generate large region." + 
					   "RegionX=" + largeRegionX + "->" + (largeRegionX + algorithm.generationRegionWidth) +
					   "RegionY=" + largeRegionY + "->" + (largeRegionY + algorithm.generationRegionHeight));

			region = new LargeRegion {
				regionX = largeRegionX,
				regionY = largeRegionY,
				regionWidth = algorithm.generationRegionWidth,
				regionHeight = algorithm.generationRegionHeight,
				tiles = algorithm.GenerateTiles (
					largeRegionX * Grid.REGION_SIZE, 
					largeRegionY * Grid.REGION_SIZE, 
					algorithm.generationRegionWidth * Grid.REGION_SIZE, 
					algorithm.generationRegionHeight * Grid.REGION_SIZE
				)
			};

			largeRegions.Add (region);
		}

		int xOffset = (regionX - region.regionX) * Grid.REGION_SIZE;
		int yOffset = (regionY - region.regionY) * Grid.REGION_SIZE;

		for (int i = 0; i < Grid.REGION_SIZE; i++) {
			for (int j = 0; j < Grid.REGION_SIZE; j++) {
				tempTiles [i, j] = region.tiles [i + xOffset, j + yOffset];
			}
		}

		return tempTiles;
	}
	public override void SaveTiles (int regionX, int regionY, FiniteGrid tiles) {

	}

	LargeRegion GetRegions(int regionX, int regionY) {
		foreach (LargeRegion generatedRegion in largeRegions) {
			if (generatedRegion.regionX <= regionX && generatedRegion.regionX + generatedRegion.regionWidth > regionX
			    && generatedRegion.regionY <= regionY && generatedRegion.regionY + generatedRegion.regionHeight > regionY) {
				return generatedRegion;
			}
		}

		return null;
	}
}