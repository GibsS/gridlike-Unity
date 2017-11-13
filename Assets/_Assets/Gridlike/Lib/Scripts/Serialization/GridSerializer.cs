using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace Gridlike {
	
	[Serializable]
	public class GridSaveManifest {

		public List<Point> regionPositions;

		public GridSaveManifest() {
			regionPositions = new List<Point> ();
		}
	}

	public class GridSerializer {

		GridSaveManifest _manifest;

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

		bool usePersistentPath;
		string path;

		public GridSerializer(bool usePersistentPath, string path) {
			this.usePersistentPath = usePersistentPath;
			this.path = path;
		}

		public bool IsRegionSaved(int regionX, int regionY) {
			if (_manifest == null) LoadManifest ();

			return _manifest.regionPositions.Contains (new Point (regionX, regionY));
		}

		public string RootPath() {
			if (usePersistentPath) {
				return Application.persistentDataPath + "/" + path;
			} else {
				return path;
			}
		}
		public string ManifestPath() {
			return RootPath () + "/manifest.data";
		}
		public string RegionPath(int X, int Y) {
			return RootPath () + "/regionX-" + X + "Y-" + Y + ".data";
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

			BinaryFormatter bf = new BinaryFormatter();
			Directory.CreateDirectory (RootPath());
			FileStream file = File.Create(RegionPath (tiles.regionX, tiles.regionY));
			bf.Serialize(file, tiles);
			file.Close();
		}

		public void SaveGrid(FiniteGrid tiles) {
			_SaveGrid (tiles);

			SaveManifest ();
		}
		public void SaveGrid(List<FiniteGrid> tiles) {
			foreach (FiniteGrid grid in tiles) {
				_SaveGrid (grid);
			}

			SaveManifest ();
		}
		public void LoadGrid(int X, int Y, FiniteGridCallback callback) {
			if (File.Exists (RegionPath(X, Y))) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (RegionPath(X, Y), FileMode.Open);

				try {
					callback(bf.Deserialize (file) as FiniteGrid);
					return;
				} catch {
					Debug.Log ("[GridSerializer] Failed to load region X=" + X + "Y=" + Y);
					callback (null);
					return;
				}
			} else {
				Debug.Log ("[GridSerializer] Region region X=" + X + "Y=" + Y + " not found");
				callback (null);
				return;
			}
		}

		public void Clear() {
			if (_manifest != null) _manifest.regionPositions.Clear ();

			if(Directory.Exists(RootPath())) {
				Directory.Delete (RootPath (), true);
			}
		}

	}
}