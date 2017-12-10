using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gridlike {

	/// <summary>
	/// A simple grid loading/saving GridDataDelegate.
	/// </summary>
	[AddComponentMenu("Gridlike/Grid saver")]
	public class GridSaver : GridDataDelegate {

		/// <summary>
		/// Is the save path specified relative to Unity's persistentDataPath?
		/// </summary>
		[SerializeField] bool _usePersistentPath;
		/// <summary>
		/// The save path.
		/// </summary>
		[SerializeField] string _path;

		/// <summary>
		/// The grid serializer.
		/// </summary>
		GridSerializer gridSerializer;

		/// <summary>
		/// Is the save path specified relative to Unity's persistentDataPath?
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
		/// The save path.
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
		/// The absolute save path.
		/// </summary>
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
			#if UNITY_WEBGL && !UNITY_EDITOR

			Debug.LogError("[Gridlike] WebGL build does not support serialization");
			Destroy(this);

			return;

			#endif

			Initialize ();
		}

		void Initialize() {
			gridSerializer = new GridSerializer (usePersistentPath, path);
		}

		/// <summary>
		/// Asynchronously loads the specified region.
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="callback">The callback to call once the region is loaded.</param>
		public override void LoadTiles (int regionX, int regionY, Action<FiniteGrid> callback) {
			if (gridSerializer.IsRegionSaved(regionX, regionY)) {
				gridSerializer.LoadGrid (regionX, regionY, callback);
				return;
			}

			callback (null);
		}
		/// <summary>
		/// Asynchronously saves the specified region.
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="tiles">The data for the region.</param>
		public override void SaveTiles (int regionX, int regionY, FiniteGrid tiles) {
			gridSerializer.SaveGrid (tiles);
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