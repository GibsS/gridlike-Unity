using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Stores meta save information about a grid. Notably the list of region positions.
	/// </summary>
	[Serializable]
	public class GridSaveManifest {

		/// <summary>
		/// Position of every regions in the save.
		/// </summary>
		public List<Point> regionPositions;

		public GridSaveManifest() {
			regionPositions = new List<Point> ();
		}
	}

	/// <summary>
	/// Handles saving and loading of a specific Grid's information.
	/// The data is saved in a folder as a list of files (one for each region) + a manifest file.
	/// </summary>
	public class GridSerializer {

		GridSaveManifest _manifest;

		/// <summary>
		/// The manifest for the save file of the Grid.
		/// </summary>
		/// <value>The manifest.</value>
		public GridSaveManifest manifest {
			get {
				if (_manifest == null) {
					LoadManifest ();
				}
				return _manifest;
			}
			set {
				_manifest = value;
			}
		}

		string persistentDataPath;
		bool usePersistentPath;
		string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="Gridlike.GridSerializer"/> class.
		/// </summary>
		/// <param name="usePersistentPath">If set to <c>true</c> the provided path is relative to Unity3D's persistentDataPath.</param>
		/// <param name="path">The path of the save.</param>
		public GridSerializer(bool usePersistentPath, string path) {
			this.persistentDataPath = Application.persistentDataPath;
			this.usePersistentPath = usePersistentPath;
			this.path = path;
		}

		/// <summary>
		/// Determines whether their is data for the specified region saved or not.
		/// </summary>
		/// <returns><c>true</c> if the region is stored in memory; otherwise, <c>false</c>.</returns>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		public bool IsRegionSaved(int regionX, int regionY) {
			if (_manifest == null) LoadManifest ();

			return _manifest.regionPositions.Contains (new Point (regionX, regionY));
		}

		/// <summary>
		/// The path of the folder of the save.
		/// </summary>
		public string RootPath() {
			if (usePersistentPath) {
				return persistentDataPath + "/" + path;
			} else {
				return path;
			}
		}
		/// <summary>
		/// The path of the manifest.
		/// </summary>
		public string ManifestPath() {
			return RootPath () + "/manifest.data";
		}
		/// <summary>
		/// The path of the save file for a given region.
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		public string RegionPath(int regionX, int regionY) {
			return RootPath () + "/regionX-" + regionX + "Y-" + regionY + ".data";
		}

		void SaveManifest() {
			BinaryFormatter bf = new BinaryFormatter();
			string p = ManifestPath ();
			Directory.CreateDirectory (RootPath());
			FileStream file = File.Create(p);
			bf.Serialize(file, _manifest);
			file.Close();
		}

		void LoadManifest() {
			if (File.Exists (ManifestPath ())) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (ManifestPath (), FileMode.Open);

				try {
					_manifest = bf.Deserialize (file) as GridSaveManifest;
				} catch {
					Debug.LogError ("[Gridlike] Failed to load grid save manifest.");
					_manifest = new GridSaveManifest ();
				}

				file.Close ();
			} else {
				_manifest = new GridSaveManifest ();
			}
		}

		void _SaveGrid(FiniteGrid tiles) {
			Point point = new Point (tiles.regionX, tiles.regionY);
			if (!_manifest.regionPositions.Contains (point)) {
				_manifest.regionPositions.Add (point);
			}
				
			ThreadManager.CreateJob(() => { Serialize(tiles); return 0; }, (fail, res) => { });
		}

		/// <summary>
		/// Save a region. Uses threading.
		/// </summary>
		/// <param name="tiles">The region tiles.</param>
		public void SaveGrid(FiniteGrid tiles) {
			_SaveGrid (tiles);

			SaveManifest ();
		}
		/// <summary>
		/// Save a list of regions. Uses threading.
		/// </summary>
		/// <param name="tiles">The regions tiles.</param>
		public void SaveGrid(List<FiniteGrid> tiles) {
			foreach (FiniteGrid grid in tiles) {
				_SaveGrid (grid);
			}

			SaveManifest ();
		}
		/// <summary>
		/// Loads a region. Uses threading.
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="callback">The callback called once the loading is done.</param>
		public void LoadGrid(int regionX, int regionY, Action<FiniteGrid> callback) {
			if (File.Exists (RegionPath(regionX, regionY))) {
				ThreadManager.CreateJob(() => Deserialize (regionX, regionY), (fail, res) => {
					callback(res);
					if(fail) callback(null); 
					else callback(res);
				});
			} else {
				Debug.Log ("[GridSerializer] Region region X=" + regionX + "Y=" + regionY + " not found");
				callback (null);
				return;
			}
		}

		/// <summary>
		/// Destroy save.
		/// </summary>
		public void Clear() {
			if (_manifest != null) _manifest.regionPositions.Clear ();

			if(Directory.Exists(RootPath())) {
				Directory.Delete (RootPath (), true);
			}
		}

		void Serialize(FiniteGrid tiles) {
			BinaryFormatter bf = new BinaryFormatter();
			Directory.CreateDirectory (RootPath());
			FileStream file = File.Create(RegionPath (tiles.regionX, tiles.regionY));
			bf.Serialize(file, tiles);
			file.Close();
		}

		FiniteGrid Deserialize(int regionX, int regionY) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (RegionPath(regionX, regionY), FileMode.Open);

			try {
				return bf.Deserialize (file) as FiniteGrid;
			} catch {
				Debug.Log ("[GridSerializer] Failed to load region X=" + regionX + "Y=" + regionY);
				return null;
			}
		}
	}
}