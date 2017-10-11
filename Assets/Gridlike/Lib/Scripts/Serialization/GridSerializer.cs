﻿﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

using UnityEngine;

[Serializable]
public class GridSaveManifest {

	public List<Point> regionPositions;

	public GridSaveManifest() {
		regionPositions = new List<Point> ();
	}
}

public class GridSerializer {

	GridSaveManifest manifest;

	string path;

	public GridSerializer(string path) {
		this.path = path;
	}

	public bool IsRegionSaved(int regionX, int regionY) {
		if (manifest == null) {
			LoadManifest ();
		}

		return manifest.regionPositions.Contains (new Point (regionX, regionY));
	}

	string ManifestPath() {
		return Application.persistentDataPath + "/" + path + "/manifest.data";
	}
	string RegionPath(int X, int Y) {
		return Application.persistentDataPath + "/" + path + "/regionX=" + X + " Y=" + Y + ".data";
	}

	void SaveManifest() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(ManifestPath());
		bf.Serialize(file, manifest);
		file.Close();
	}

	void LoadManifest() {
		if (File.Exists (ManifestPath ())) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (ManifestPath (), FileMode.Open);

			try {
				manifest = bf.Deserialize (file) as GridSaveManifest;
			} catch {
				Debug.Log ("[GridSerializer] Failed to load manifest.");
				manifest = new GridSaveManifest ();
			}
		} else {
			Debug.Log ("[GridSerializer] No manifest found.");
			manifest = new GridSaveManifest ();
		}
	}

	void _SaveGrid(FiniteGrid tiles) {
		manifest.regionPositions.Add (new Point (tiles.regionX, tiles.regionY));

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(RegionPath(tiles.regionX, tiles.regionY));
		bf.Serialize(file, manifest);
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
	public FiniteGrid LoadGrid(int X, int Y) {
		if (File.Exists (RegionPath(X, Y))) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (RegionPath(X, Y), FileMode.Open);

			try {
				return bf.Deserialize (file) as FiniteGrid;
			} catch {
				Debug.Log ("[GridSerializer] Failed to load region X=" + X + "Y=" + Y);
				return null;
			}
		} else {
			Debug.Log ("[GridSerializer] Region region X=" + X + "Y=" + Y + " not found");
			return null;
		}
	}

	public void Clear() {
		foreach (Point point in manifest.regionPositions) {
			File.Delete (RegionPath (point.x, point.y));
		}
		File.Delete (ManifestPath ());
		manifest.regionPositions.Clear ();
	}
}