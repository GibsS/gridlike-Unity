using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gridlike {

	/// <summary>
	/// A simple positionned two dimensional array. (positionned in region space).
	/// </summary>
	public class LargeRegion {

		public int regionX;
		public int regionY;
		public int regionWidth;
		public int regionHeight;

		public Tile[,] tiles;
	}

	/// <summary>
	/// GridDataDelegate that handles procedural generation using an algorithm attached to the Grid.
	/// </summary>
	[AddComponentMenu("Gridlike/Grid generator")]
	public class GridGenerator : GridDataDelegate {

		/// <summary>
		/// The width in regions of a generated area.
		/// </summary>
		public int generationRegionWidth;
		/// <summary>
		/// The width in regions of a generated area.
		/// </summary>
		public int generationRegionHeight;

		/// <summary>
		/// Is the save path specified relative to Unity's persistentDataPath? Not to be modified in play mode.
		/// </summary>
		[SerializeField] bool _usePersistentPath;
		/// <summary>
		/// The save path. Ignored if useSave is false. Not to be modified in play mode.
		/// </summary>
		[SerializeField] string _path;

		/// <summary>
		/// Use serialization. Not to be modified in play mode.
		/// </summary>
		public bool useSave;

		/// <summary>
		/// The grid serializer.
		/// </summary>
		GridSerializer gridSerializer;

		/// <summary>
		/// The large regions.
		/// </summary>
		List<LargeRegion> largeRegions;

		/// <summary>
		/// The associated generation algorithm.
		/// </summary>
		public GridGeneratorAlgorithm algorithm;

		/// <summary>
		/// Is the path specified relative to Unity's persistentDataPath?
		/// </summary>
		public bool usePersistentPath {
			get { return _usePersistentPath; }
			set {
				bool old = _usePersistentPath;
				_usePersistentPath = value;

				if (old != value) Initialize ();
			}
		}
		/// <summary>
		/// The save path. Ignored if useSave is false.
		/// </summary>
		public string path {
			get { return _path; }
			set {
				string old = _path;
				_path = value;

				if (old != value) Initialize ();
			}
		}
		/// <summary>
		/// The absolute path to the save folder.
		/// </summary>
		public string rootPath { 
			get { 
				if (gridSerializer == null) Initialize ();

				return gridSerializer.RootPath(); 
			} 
		}

		/// <summary>
		/// The save manifest.
		/// </summary>
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

			#if UNITY_WEBGL && !UNITY_EDITOR

			useSave = false;

			#endif

			largeRegions = new List<LargeRegion> ();

			if(useSave) gridSerializer = new GridSerializer (usePersistentPath, path);
		}

		/// <summary>
		/// Reset by fetching the algorithm again.
		/// </summary>
		public override void ResetDelegate() {
			base.ResetDelegate ();

			if (algorithm == null && !gettingDestroyed) {
				algorithm = GetComponent<GridGeneratorAlgorithm> ();
			}
		}
		/// <summary>
		/// Sets the algorithm, to be called by the GridGeneratorAlgorithm.
		/// </summary>
		public void _SetAlgorithm(GridGeneratorAlgorithm algorithm) {
			this.algorithm = algorithm;
		}

		/// <summary>
		/// Asynchronously returns the data for a region through the callback. The tiles are either loaded if loading is enabled or generated if loading
		/// is not used or that region has not yet been loaded.
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="callback">The callback to call once the region is loaded.</param>
		public override void LoadTiles (int regionX, int regionY, Action<FiniteGrid> callback) {
			if (useSave && gridSerializer.IsRegionSaved(regionX, regionY)) {
				gridSerializer.LoadGrid (regionX, regionY, callback);
				return;
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

			FiniteGrid region2 = new FiniteGrid(regionX, regionY, Grid.REGION_SIZE);

			bool empty = true;
			for (int i = 0; i < Grid.REGION_SIZE; i++) {
				for (int j = 0; j < Grid.REGION_SIZE; j++) {
					Tile tile = largeRegion.tiles [xOffset + i, yOffset + j];

					if (tile.id > 0) empty = false;

					region2.Set(i, j, tile);
				}
			}

			if (empty) {
				callback (null);
			} else {
				callback (region2);
			}
		}
		/// <summary>
		/// Saves the region data if saving is enabled. Ignores otherwise.
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="tiles">The data for the region.</param>
		public override void SaveTiles (int regionX, int regionY, FiniteGrid tiles) {
			if (useSave) {
				gridSerializer.SaveGrid (tiles);
			}
		}

		LargeRegion GetRegions(int regionX, int regionY) {
			if (largeRegions == null) largeRegions = new List<LargeRegion> ();
			
			foreach (LargeRegion generatedRegion in largeRegions) {
				if (generatedRegion.regionX <= regionX && generatedRegion.regionX + generatedRegion.regionWidth > regionX
				    && generatedRegion.regionY <= regionY && generatedRegion.regionY + generatedRegion.regionHeight > regionY) {
					return generatedRegion;
				}
			}

			return null;
		}

		/// <summary>
		/// Clears the save.
		/// </summary>
		public void ClearSave() {
			if (gridSerializer == null) Initialize ();

			gridSerializer.Clear ();
		}
	}
}