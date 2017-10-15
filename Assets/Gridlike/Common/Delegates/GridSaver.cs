using UnityEngine;
using System.Collections.Generic;

public class GridSaver : GridDataDelegate {

	[SerializeField] bool _usePersistentPath;
	[SerializeField] string _path;

	GridSerializer gridSerializer;

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
		gridSerializer = new GridSerializer (usePersistentPath, path);
	}

	public override FiniteGrid LoadTiles (int regionX, int regionY) {
		if (gridSerializer.IsRegionSaved(regionX, regionY)) {
			FiniteGrid grid = gridSerializer.LoadGrid (regionX, regionY);

			if (grid != null) {
				Debug.Log ("Load region from save. X=" + regionX + " Y=" + regionY);
				return grid;
			}
		}

		return null;
	}
	public override void SaveTiles (int regionX, int regionY, FiniteGrid tiles) {
		gridSerializer.SaveGrid (tiles);

		Debug.Log ("Save region. X=" + regionX + " Y=" + regionY);
	}

	public void ClearSave() {
		if (gridSerializer == null) Initialize ();

		gridSerializer.Clear ();
	}
}