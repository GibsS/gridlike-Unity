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

	[SerializeField] bool _usePersistentPath;
	[SerializeField] string _path;

	public bool useSave;

	GridSerializer gridSerializer;

	List<LargeRegion> largeRegions;

	[SerializeField] GridGeneratorAlgorithm algorithm;

	public bool usePersistentPath {
		get { return _usePersistentPath; }
		set {
			bool old = _usePersistentPath;
			_usePersistentPath = value;

			if (old != value) Initialize ();
		}
	}
	public string path {
		get { return _path; }
		set {
			string old = _path;
			_path = value;

			if (old != value) Initialize ();
		}
	}
	public string rootPath { 
		get { 
			if (gridSerializer == null) Initialize ();

			return gridSerializer.RootPath(); 
		} 
	}

	public GridSaveManifest gridSaveManifest { 
		get { 
			if (gridSerializer == null) Initialize ();
			return gridSerializer.manifest; 
		} 
	}

	void Start() {
		Initialize ();
	}

	void Initialize() {
		largeRegions = new List<LargeRegion> ();

		if(useSave) gridSerializer = new GridSerializer (usePersistentPath, path);
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

	// TODO handle dataPresent
	public override FiniteGrid LoadTiles (bool dataPresent, int regionX, int regionY) {
		LargeRegion largeRegion = GetRegions (regionX, regionY);

		if (largeRegion == null) {
			Debug.Log ("Try to load: useSave? " + useSave + " is region saved? " + gridSerializer.IsRegionSaved (regionX, regionY));
			if (useSave && gridSerializer.IsRegionSaved(regionX, regionY)) {
				FiniteGrid grid = gridSerializer.LoadGrid (regionX, regionY);

				if (grid != null) {
					Debug.Log ("Load region from save. X=" + regionX + " Y=" + regionY);
					return grid;
				}
			}

			int largeRegionX = Mathf.FloorToInt (regionX / (float)algorithm.generationRegionWidth) * algorithm.generationRegionWidth;
			int largeRegionY = Mathf.FloorToInt (regionY / (float)algorithm.generationRegionHeight) * algorithm.generationRegionHeight;

			Debug.Log ("Generate large region." + 
					   "RegionX=" + largeRegionX + "->" + (largeRegionX + algorithm.generationRegionWidth) +
					   "RegionY=" + largeRegionY + "->" + (largeRegionY + algorithm.generationRegionHeight));

			largeRegion = new LargeRegion {
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

			largeRegions.Add (largeRegion);
		}

		int xOffset = (regionX - largeRegion.regionX) * Grid.REGION_SIZE;
		int yOffset = (regionY - largeRegion.regionY) * Grid.REGION_SIZE;

		FiniteGrid region = new FiniteGrid(regionX, regionY, Grid.REGION_SIZE);

		bool empty = true;
		for (int i = 0; i < Grid.REGION_SIZE; i++) {
			for (int j = 0; j < Grid.REGION_SIZE; j++) {
				Tile tile = largeRegion.tiles [i + xOffset, j + yOffset];

				if (tile.id > 0) empty = false;

				region.Set(i, j, tile);
			}
		}

		Debug.Log ("Load region from generation. X=" + regionX + " Y=" + regionY);

		if (empty) {
			return null;
		} else {
			return region;
		}
	}
	public override void SaveTiles (int regionX, int regionY, FiniteGrid tiles) {
		if (useSave) {
			gridSerializer.SaveGrid (tiles);

			Debug.Log ("Save region. X=" + regionX + " Y=" + regionY);
		}
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

	public void ClearSave() {
		if (gridSerializer == null) Initialize ();

		gridSerializer.Clear ();
	}
}