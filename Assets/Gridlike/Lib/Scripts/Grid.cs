using System;
using System.Collections.Generic;
using UnityEngine;

// TODO Add namespaces everywhere
// TODO Put all classes in individual files
// TODO Agree on final naming scheme
// TODO Comment
// TODO Remove any residual Debug.Log

// TODO make grid factories (static in Grid)
// TODO make sure grids can be created from code easily (define stories)

// TODO In scene view, show information about hovered tile

// TODO If progressive loading, make sure to "hide" every tile before play + when leaving editor
// TODO If inspector values should not change during play mode, hide?

// TODO Add singleton behaviour to handle accessing list of currently present grids

// TODO GO pinning
// TODO Allow regular gizmos to be accessed even when the grid is selected
// TODO Custom editor for every common listener and data delegate (+ if modifying field is ignored, make them unmodifiable)
// TODO Grid updater component
// TODO Make sure sets on region that aren't presented don't call listeners
// TODO Add Gridlike component menu to pick your own grid components

// TODO Sprite renderer should pick sorting layer
// TODO Add sprite sorting layer (defined in atlas)
// TODO Add isSensor + layer on collider (defined in atlas)

// TODO Add user data 
// TODO Use kenney tiles for testing + samples 
[ExecuteInEditMode]
[AddComponentMenu("Gridlike/Grid")]
public class Grid : MonoBehaviour {

	public const int REGION_SIZE = 50;

	[SerializeField] GridDataDelegate gridDelegate;
	[SerializeField] List<GridListener> gridListeners;

	// TODO optimize
	[SerializeField] InfiniteGrid tiles;

	[SerializeField] float _tileSize = 1;

	[HideInInspector] public TileAtlas atlas;

	public bool useLoading;
	public bool useAgentBasedLoading;
	GridAgentLoadPolicy loadPolicy;

	public bool saveOnClose;

	public float tileSize {
		get { return _tileSize; }
		set { if(value != _tileSize) SetTileSize (value); }
	}

	void Init() {
		if (tiles == null) {
			gridListeners = new List<GridListener> ();

			tiles = new InfiniteGrid (REGION_SIZE);

			foreach (GridListener listener in GetComponents<GridListener> ()) {
				listener.ResetListener ();
			}

			gridDelegate = GetComponent<GridDataDelegate> ();
		}
	}

	#region UNITY EVENTS

	void Reset() {
		tiles = null;

		Init ();
	}
	void Awake() {
		Init ();
	}

	void OnDestroy() {
		foreach (GridListener listener in GetComponents<GridListener> ()) {
			listener.ResetListener ();
		}

		if (Application.isPlaying && saveOnClose) {
			SaveAllRegion ();
		}
	}

	void Update() {
		if (useLoading && useAgentBasedLoading) {
			if (loadPolicy == null) loadPolicy = new GridAgentLoadPolicy (this);

			loadPolicy.Update();
		}
	}

	#endregion 

	#region GRID DATA DELEGATE

	public void SetDelegate(GridDataDelegate gridDelegate) {
		this.gridDelegate = gridDelegate;
	}

	#endregion

	#region GRID LISTENERS

	public void AddListener(GridListener listener) {
		if (!gridListeners.Contains (listener)) {
			gridListeners.Add (listener);
		}
	}
	public void RemoveListener(GridListener listener) {
		if (gridListeners.Contains (listener)) {
			gridListeners.Remove (listener);
		}
	}

	#endregion

	#region REGION PRESENTING/LOADING

	public void PresentAllAgain() {
		List<FiniteGrid> regions = tiles.GetRegions ().FindAll(r => r.presented);
		foreach (FiniteGrid regionPosition in regions) {
			HideRegion (regionPosition.regionX, regionPosition.regionY);
		}

		foreach (FiniteGrid regionPosition in regions) {
			PresentRegion (regionPosition.regionX, regionPosition.regionY);
		}
	}

	public void PresentAll() {
		foreach (FiniteGrid region in tiles.GetRegions().FindAll(r => r.presented)) {
			PresentRegion (region.regionX, region.regionY);
		}
	}
	public void HideAll() {
		foreach (FiniteGrid region in tiles.GetRegions().FindAll(r => r.presented)) {
			HideRegion (region.regionX, region.regionY);
		}
	}

	public void PresentRegion(int X, int Y) {
		FiniteGrid grid = tiles.GetRegion (X, Y);

		if (grid == null) {
			LoadRegion (X, Y);

			grid = tiles.GetRegion (X, Y);
		}

		if(grid != null && !grid.presented) {
			_PresentRegion (X, Y, grid);
		}
	}
	public void HideRegion(int X, int Y) {
		FiniteGrid grid = tiles.GetRegion (X, Y);

		if(grid != null && grid.presented) {
			_HideRegion (X, Y, grid);
		}
	}

	public void LoadRegion(int X, int Y) {
		if (gridDelegate != null) {
			FiniteGrid region = tiles.GetRegion (X, Y);

			if (region == null) {
				region = gridDelegate.LoadTiles (X, Y);

				if (region != null) {
					for (int i = 0; i < Grid.REGION_SIZE; i++) {
						for (int j = 0; j < Grid.REGION_SIZE; j++) {
							Tile tile = region.Get (i, j);
							tile.shape = atlas.GetTile (tile.id).shape;
						}
					}

					tiles.SetRegion (X, Y, region);
				}
			}
		}
	}
	public void SaveAllRegion() {
		foreach (FiniteGrid region in GetRegions()) {
			SaveRegion (region.regionX, region.regionY);
		}
	}
	public void SaveRegion(int X, int Y, bool unload = false) {
		if (gridDelegate != null) {
			FiniteGrid region = tiles.GetRegion (X, Y);

			if (region != null) {
				gridDelegate.SaveTiles (X, Y, region);
			}
		}

		if (unload) {
			UnloadRegion (X, Y);
		}
	}
	public void UnloadRegion(int X, int Y) {
		FiniteGrid grid = GetRegion (X, Y);

		if(grid != null) {
			_HideRegion (X, Y, grid);

			tiles.ClearRegion (X, Y);
		}
	}

	void _PresentRegion(int X, int Y, FiniteGrid grid) {
		foreach (GridListener listener in gridListeners) {
			listener.OnShowRegion (X, Y);
		}

		grid.presented = true;
	}
	void _HideRegion(int X, int Y, FiniteGrid grid) {
		foreach (GridListener listener in gridListeners) {
			listener.OnHideRegion (X, Y);
		}

		grid.presented = false;
	}

	#endregion

	#region REGION GET/SET

	public FiniteGrid GetRegion(int X, int Y) {
		return tiles.GetRegion (X, Y);
	}

	public FiniteGrid GetContainingRegion(int x, int y) {
		return tiles.GetContainingRegion (x, y);
	}

	public IEnumerable<FiniteGrid> GetRegions() {
		return tiles.GetRegions ();
	}

	#endregion

	#region TILE GET/SET

	public Tile Get(int x, int y) {
		return tiles.Get (x, y) as Tile;
	}
	public TileShape GetShape(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? TileShape.EMPTY : tile.shape;
	}
	public int GetId(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? 0 : tile.id;
	}
	public int GetSubId(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? 0 : tile.subId;
	}

	public void Set(int x, int y, int id, int subid, int state1, int state2, int state3) {
		Tile tile = GetOrCreate (x, y);

		tile.shape = atlas.GetTile(id).shape;

		tile.id = id;
		tile.subId = subid;

		tile.state1 = state1;
		tile.state2 = state2;
		tile.state3 = state3;

		foreach (GridListener listener in gridListeners) {
			listener.OnSet (x, y, tile);
		}
	}
	public void SetId(int x, int y, int id, int subId = int.MinValue) {
		Tile tile = GetOrCreate (x, y);
		int oldId = tile.id, oldSubId = tile.subId;

		tile.id = id;
		tile.shape = atlas.GetTile(id).shape;
		if (subId != int.MinValue) tile.subId = subId;

		foreach (GridListener listener in gridListeners) {
			listener.OnSetId (x, y, tile, oldId, oldSubId);
		}
	}
	public void SetSubId(int x, int y, int subId) {
		Tile tile = GetOrCreate (x, y);
		int oldId = tile.id, oldSubId = tile.subId;

		tile.subId = subId;

		foreach (GridListener listener in gridListeners) {
			listener.OnSetId (x, y, tile, oldId, oldSubId);
		}
	}
	public void SetState(int x, int y, float state1, float state2 = float.NegativeInfinity, float state3 = float.NegativeInfinity) {
		Tile tile = GetOrCreate (x, y);

		float oldState1 = tile.state1, oldState2 = tile.state2, oldState3 = tile.state3;

		tile.state1 = state1;
		if (state2 != float.NegativeInfinity) tile.state2 = state2;
		if (state3 != float.NegativeInfinity) tile.state3 = state3;

		foreach (GridListener listener in gridListeners) {
			listener.OnSetState (x, y, tile, oldState1, oldState2, oldState3);
		}
	}
		
	Tile GetOrCreate(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		if (tile == null) {
			tile = new Tile ();

			tiles.Set (x, y, tile);
		}

		return tile;
	}

	#endregion

	#region TILE SIZE

	public void SetTileSize(float tileSize) {
		this._tileSize = tileSize;

		foreach (GridListener listeners in gridListeners) {
			listeners.OnTileSizeChange ();
		}
	}

	#endregion

	#region REFERENTIAL

	public Vector2 TileCenterInWorld(int x, int y) {
		return transform.TransformPoint (TileCenterInTransform (x, y));
	}
	public Vector2 TileCenterInTransform(int x, int y) {
		return new Vector2 ((x + 0.5f) * _tileSize, (y + 0.5f) * _tileSize); 
	}
	public Vector2 TileSpaceToTransform(float x, float y) {
		return new Vector2 (x * _tileSize, y * _tileSize);
	}

	public void WorldToGrid(Vector2 position, out int x, out int y) {
		position = transform.InverseTransformPoint (position);

		TransformToGrid (position, out x, out y);
	}

	public void TransformToGrid(Vector2 position, out int x, out int y) {
		x = Mathf.FloorToInt(((float)position.x) / tileSize);
		y = Mathf.FloorToInt(((float)position.y) / tileSize);
	}

	#endregion
}