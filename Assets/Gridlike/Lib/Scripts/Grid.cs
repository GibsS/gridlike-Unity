﻿using System;
using System.Collections.Generic;
using UnityEngine;

// LATER
// 7: Grid updater 1 day
// TODO Grid updater component

// 5: Create edition palette 2 day
// TODO In grid editor add show all option with a warning when too many regions are shown

// 6: Factories + Singleton 1 day
// TODO make grid factories (static in Grid)
// TODO make sure grids can be created from code easily (define stories)
// TODO make it possible to create a grid directly out of a small two dimensional array of tiles

// TODO Add singleton behaviour to handle accessing list of currently present grids

// 8: Finish optimizing sprite renderer 2 day
// TODO Add the drag and drop if possible

// TODO Add namespaces everywhere
// TODO Put all classes in individual files
// TODO Agree on final naming scheme
// TODO Comment
// TODO Remove any residual Debug.Log

// TODO Use kenney tiles for testing + samples 
// TODO Create samples + test

// BUG Allow regular gizmos to be accessed even when the grid is selected
// TODO Handle subids

[ExecuteInEditMode]
[AddComponentMenu("Gridlike/Grid")]
public class Grid : MonoBehaviour {

	public const int REGION_SIZE = 50;

	[SerializeField] GridDataDelegate gridDelegate;
	[SerializeField] List<GridListener> gridListeners;

	[SerializeField] InfiniteGrid tiles;
	[SerializeField] InfiniteTileGOGrid tileGOs;

	[SerializeField] float _tileSize;

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
			gridDelegate = GetComponent<GridDataDelegate> ();
			gridListeners = new List<GridListener> ();

			tiles = new InfiniteGrid (REGION_SIZE);
			tileGOs = new InfiniteTileGOGrid (gameObject, REGION_SIZE);

			_tileSize = 1;

			foreach (GridListener listener in GetComponents<GridListener> ()) {
				listener.ResetListener ();
			}
		}
	}

	#region UNITY EVENTS

	void Reset() {
		if(tiles != null) HideAll ();

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

			foreach (FiniteGrid region in tiles.GetRegions()) {
				if (region.presented) {
					listener.OnShowRegion (region.regionX, region.regionY);
				}
			}
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
		foreach (FiniteGrid region in tiles.GetRegions()) {
			PresentRegion (region.regionX, region.regionY);
		}
	}
	public void HideAll() {
		foreach (FiniteGrid region in tiles.GetRegions().FindAll(r => r.presented)) {
			HideRegion (region.regionX, region.regionY);
		}
	}

	public void PresentContainingRegion(int x, int y) {
		PresentRegion(Mathf.FloorToInt(x / (float) Grid.REGION_SIZE), Mathf.FloorToInt(y / (float) Grid.REGION_SIZE));
	}
	public void HideContainingRegion(int x, int y) {
		HideRegion(Mathf.FloorToInt(x / (float) Grid.REGION_SIZE), Mathf.FloorToInt(y / (float) Grid.REGION_SIZE));
	}

	public void PresentRegion(int X, int Y) {
		FiniteGrid grid = tiles.GetRegion (X, Y);

		if (grid == null || !grid.presented) {

			if (grid == null) {
				LoadRegion (X, Y);

				grid = tiles.GetRegion (X, Y);
			}

			if (grid != null) {
				_PresentRegion (X, Y, grid);
			}
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
	public void UnloadContainingRegion(int x, int y) {
		UnloadRegion(Mathf.FloorToInt(x / (float) Grid.REGION_SIZE), Mathf.FloorToInt(y / (float) Grid.REGION_SIZE));
	}
	public void UnloadRegion(int X, int Y) {
		FiniteGrid grid = GetRegion (X, Y);

		if(grid != null) {
			if (grid.presented) {
				_HideRegion (X, Y, grid);
			}

			tiles.ClearRegion (X, Y);
		}
	}

	void _PresentRegion(int X, int Y, FiniteGrid grid) {
		foreach (GridListener listener in gridListeners) {
			listener.OnShowRegion (X, Y);
		}

		int bx = X * REGION_SIZE, by = Y * REGION_SIZE;

		for (int i = 0; i < REGION_SIZE; i++) {
			for (int j = 0; j < REGION_SIZE; j++) {
				Tile tile = grid.Get (i, j);

				if (tile != null && tile.tileGOCenter) {
					if (!tileGOs.TryCreateTileGO (atlas [tile.id], bx + i, by + j, (xx, yy) => { })) {
						tile.tileGOCenter = false;
						tile.id = 0;
					}
				}
			}
		}

		grid.presented = true;
	}
	void _HideRegion(int X, int Y, FiniteGrid grid) {
		foreach (GridListener listener in gridListeners) {
			listener.OnHideRegion (X, Y);
		}

		int bx = X * REGION_SIZE, by = Y * REGION_SIZE;

		for (int i = 0; i < REGION_SIZE; i++) {
			for (int j = 0; j < REGION_SIZE; j++) {
				Tile tile = grid.Get (i, j);

				if (tile != null && tile.tileGOCenter) {
					tileGOs.DestroyTileGO (bx + i, by + j, (xx, yy) => { });
				}
			}
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

		return tile == null ? TileShape.EMPTY : atlas[tile.id].shape;
	}
	public int GetId(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? 0 : tile.id;
	}
	public int GetSubId(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? 0 : tile.subId;
	}
	public Component GetTileComponent(int x, int y) {
		return tileGOs.GetComponent (x, y);
	}

	public bool CanSet(int x, int y, int id) {
		TileInfo info = atlas [id];
		Tile tile = tiles.Get (x, y);

		if (tile == null) return true;

		if (info.tileGO == null) {
			return tile.tileGOCenter || tileGOs.GetTileGO (x, y) == null;
		} else {
			TileBehaviour behaviour = info.tileGO.GetComponent<TileBehaviour> ();

			if (behaviour == null) {
				return tile.tileGOCenter || tileGOs.GetTileGO (x, y) == null;
			} else {
				return !tileGOs.HasOverlap (x, y, behaviour);
			}
		}
	}
		
	public void Set(int x, int y, int id, int subid, float state1, float state2, float state3) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);

		_Set (tile, x, y, id, subid, state1, state2, state3, region.presented);
	}
	public void SetId(int x, int y, int id, int subId = -1) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);

		_Set (tile, x, y, id, subId, tile.state1, tile.state2, tile.state3, region.presented);
	}
	public void SetSubId(int x, int y, int subId) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);
		int oldSubId = tile.subId;

		tile.subId = subId;

		if (region.presented) {
			foreach (GridListener listener in gridListeners) {
				listener.OnSetSubId (x, y, tile, oldSubId);
			}
		}
	}
	public void SetState(int x, int y, float state1, float state2 = float.NegativeInfinity, float state3 = float.NegativeInfinity) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);

		float oldState1 = tile.state1, oldState2 = tile.state2, oldState3 = tile.state3;

		tile.state1 = state1;
		if (state2 != float.NegativeInfinity) tile.state2 = state2;
		if (state3 != float.NegativeInfinity) tile.state3 = state3;

		if (region.presented) {
			foreach (GridListener listener in gridListeners) {
				listener.OnSetState (x, y, tile, oldState1, oldState2, oldState3);
			}
		}
	}
	public void Clear(int x, int y) {
		Tile tile = tiles.Get (x, y);

		if (tile != null) {
			_Clear (tile, x, y);
		}
	}

	void _Clear(Tile tile, int x, int y) {
		Component component = tileGOs.GetComponent (x, y);

		if (component == null) {
			tile.id = 0;
			tile.subId = -1;

			// POTENTIAL BUG : make sure it can be called even if we don't check it's presented
			foreach (GridListener listener in gridListeners) {
				listener.OnSet (x, y, tile);
			}
		} else {
			if (component is TileBehaviour) {
				x = (component as TileBehaviour).x;
				y = (component as TileBehaviour).y; 
			}

			tileGOs.DestroyTileGO (x, y, (xx, yy) => {
				Tile other = tiles.Get(xx, yy);

				if(other != null) {
					other.id = 0;
					other.subId = -1;

					// POTENTIAL BUG : make sure it can be called even if we don't check it's presented
					foreach (GridListener listener in gridListeners) {
						listener.OnSet (xx, yy, other);
					}
				}
			});

			if (tile.tileGOCenter) {
				tile.tileGOCenter = false;
			} else {
				Get (x, y).tileGOCenter = false;
			}
		}
	}

	void _Set(Tile tile, int x, int y, int id, int subid, float state1, float state2, float state3, bool show) {
		TileInfo info = atlas.GetTile (id);

		if (tile.tileGOCenter) {
			_Clear (tile, x, y);
		} else if (tileGOs.GetTileGO (x, y) != null) {
			Debug.LogError ("[Grid] Impossible to place a tile here: a tile GO occupies this tile");
			return;
		}

		if (show) {
			if (info.tileGO != null) {
				if (!tileGOs.TryCreateTileGO (info, x, y, (xx, yy) => {
					Tile otherTile = tiles.Get (xx, yy);

					if (otherTile != null) {
						otherTile.id = 0;
						otherTile.subId = -1;

						foreach (GridListener listener in gridListeners) {
							listener.OnSet (xx, yy, otherTile);
						}
					}
				})) {
					Debug.LogError ("[Grid] Impossible to place a tile GO here: a tile GO already occupies these tiles");
					return;
				}

				tile.id = id;
				tile.subId = subid;

				tile.tileGOCenter = true;
			} else {
				tile.id = id;
				tile.subId = subid;

				foreach (GridListener listener in gridListeners) {
					listener.OnSet (x, y, tile);
				}
			}
		}

		tile.id = id;
		tile.tileGOCenter = info.tileGO != null;
		tile.subId = subid;

		tile.state1 = state1;
		tile.state2 = state2;
		tile.state3 = state3;
	}

	Tile GetOrCreate(int x, int y, out FiniteGrid region) {
		Tile tile = tiles.Get (x, y, out region) as Tile;

		if (tile == null) {
			tile = new Tile ();

			region = tiles.Set (x, y, tile);
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