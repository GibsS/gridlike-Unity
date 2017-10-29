using UnityEngine;
using System.Collections.Generic;

namespace Gridlike {
	
	public class LargeRegion {

		public int regionX;
		public int regionY;
		public int regionWidth;
		public int regionHeight;

		public Tile[,] tiles;
	}

	[AddComponentMenu("Gridlike/Grid generator")]
	public class GridGenerator : GridDataDelegate {

		public int generationRegionWidth;
		public int generationRegionHeight;

		[SerializeField] bool _usePersistentPath;
		[SerializeField] string _path;

		public bool useSave;

		GridSerializer gridSerializer;

		List<LargeRegion> largeRegions;

		public GridGeneratorAlgorithm algorithm;

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

			if (algorithm == null && !gettingDestroyed) {
				algorithm = GetComponent<GridGeneratorAlgorithm> ();
			}
		}
		public void SetAlgorithm(GridGeneratorAlgorithm algorithm) {
			this.algorithm = algorithm;
		}

		public override FiniteGrid LoadTiles (int regionX, int regionY) {
			if (useSave && gridSerializer.IsRegionSaved(regionX, regionY)) {
				FiniteGrid grid = gridSerializer.LoadGrid (regionX, regionY);
				return grid;
			}

			LargeRegion largeRegion = GetRegions (regionX, regionY);

			if (largeRegion == null) {

				int largeRegionX = Mathf.FloorToInt (regionX / (float)generationRegionWidth) * generationRegionWidth;
				int largeRegionY = Mathf.FloorToInt (regionY / (float)generationRegionHeight) * generationRegionHeight;

				largeRegion = new LargeRegion {
					regionX = largeRegionX,
					regionY = largeRegionY,
					regionWidth = generationRegionWidth,
					regionHeight = generationRegionHeight,
					tiles = algorithm.GenerateTiles (
						largeRegionX * Grid.REGION_SIZE, 
						largeRegionY * Grid.REGION_SIZE, 
						generationRegionWidth * Grid.REGION_SIZE, 
						generationRegionHeight * Grid.REGION_SIZE
					)
				};

				largeRegions.Add (largeRegion);
				if (largeRegions.Count > 15) {
					largeRegions.RemoveRange (0, 7);
				}
			}

			int xOffset = (regionX - largeRegion.regionX) * Grid.REGION_SIZE;
			int yOffset = (regionY - largeRegion.regionY) * Grid.REGION_SIZE;

			FiniteGrid region = new FiniteGrid(regionX, regionY, Grid.REGION_SIZE);

			bool empty = true;
			for (int i = 0; i < Grid.REGION_SIZE; i++) {
				for (int j = 0; j < Grid.REGION_SIZE; j++) {
					Tile tile = largeRegion.tiles [xOffset + i, yOffset + j];

					if (tile.id > 0) empty = false;

					region.Set(i, j, tile);
				}
			}

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
}