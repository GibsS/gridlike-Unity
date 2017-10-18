using System;
using System.Collections.Generic;
using UnityEngine;

// 1: Small change on custom inspectors + BUG FIX 2-3 days
// BUG Reset on grid doesn't clear colliders

// BUG Allow regular gizmos to be accessed even when the grid is selected
// BUG Weird none showing regions when using agent based loading

// 2: GO tile 2 days
// TODO GO tile
// TODO Add Clear function

// 3: finalize physics 1 day
// TODO Add isSensor + layer on collider (defined in atlas)

// 4: Increased data storage + editing 1 day
// TODO Add user data 

// 5: Create edition palette 2 day
// TODO In scene view, show information about hovered tile
// TODO Use the tilemap guys way of rendering a window
// TODO Show hide a specific region

// 6: Factories + Singleton 1 day
// TODO make grid factories (static in Grid)
// TODO make sure grids can be created from code easily (define stories)

// TODO Add singleton behaviour to handle accessing list of currently present grids

// 7: Grid updater 1 day
// TODO Grid updater component

// 8: Finish optimizing sprite renderer 2 day
// TODO Add the drag and drop if possible

// TODO Add namespaces everywhere
// TODO Put all classes in individual files
// TODO Agree on final naming scheme
// TODO Comment
// TODO Remove any residual Debug.Log

// TODO Use kenney tiles for testing + samples 
// TODO Create samples + test


[ExecuteInEditMode]
[AddComponentMenu("Gridlike/Grid")]
public class Grid : MonoBehaviour {

	public const int REGION_SIZE = 50;

	[SerializeField] GridDataDelegate gridDelegate;
	[SerializeField] List<GridListener> gridListeners;

	[SerializeField] InfiniteGrid tiles;

	[SerializeField] InfiniteComponentGrid tileGOs;
	[SerializeField] GameObject tileGOContainer;

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

			tileGOs = new InfiniteComponentGrid (REGION_SIZE);
			tileGOContainer = new GameObject ("tile container");
			tileGOContainer.transform.SetParent (transform, false);

			foreach (GridListener listener in GetComponents<GridListener> ()) {
				listener.ResetListener ();
			}

			gridDelegate = GetComponent<GridDataDelegate> ();
		}
	}

	#region UNITY EVENTS

	void Reset() {
		if(tiles != null) HideAll ();

		if (tileGOContainer != null) {
			foreach (Transform t in tileGOContainer.transform) {
				Destroy (t.gameObject);
			}
		}

		foreach (Transform t in gameObject.transform) {
			if (t.gameObject.name == "tile container") {
				DestroyImmediate (t.gameObject);
			}
		}

		DestroyImmediate (tileGOContainer);

		tiles = null;

		tileGOs = null;

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

		return tile == null ? TileShape.EMPTY : atlas.atlas[tile.id].shape;
	}
	public int GetId(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? 0 : tile.id;
	}
	public int GetSubId(int x, int y) {
		Tile tile = tiles.Get (x, y) as Tile;

		return tile == null ? 0 : tile.subId;
	}

	public Component GetTileBehaviour(int x, int y, TileBehaviour otherBehaviour = null) {
		if (otherBehaviour == null) {
			Component c = tileGOs.Get (x, y);

			if (c != null) return c;
		} else {
			bool[,] area = otherBehaviour.area;

			for (int i = 0; i < area.GetLength (0); i++) {
				for (int j = 0; j < area.GetLength (1); j++) {
					if (area [i, j]) {
						Component c = tileGOs.Get (otherBehaviour.areaBottomLeftXOffset + x + i, otherBehaviour.areaBottomLeftYOffset + y + j);

						if (c != null) return c;
					}
				}
			}
		}

		return null;
	}
		
	public void Set(int x, int y, int id, int subid, int state1, int state2, int state3) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);

		_Set (tile, x, y, id, subid, state1, state2, state3);

		if (region.presented) {
			foreach (GridListener listener in gridListeners) {
				listener.OnSet (x, y, tile);
			}
		}
	}
	public void SetId(int x, int y, int id, int subId = int.MinValue) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);

		int oldId = tile.id;
		int oldSubId = tile.subId;

		_Set (tile, x, y, id, subId, tile.state1, tile.state2, tile.state3);

		if (region.presented) {
			foreach (GridListener listener in gridListeners) {
				listener.OnSetId (x, y, tile, oldId, oldSubId);
			}
		}
	}
	public void SetSubId(int x, int y, int subId) {
		FiniteGrid region;
		Tile tile = GetOrCreate (x, y, out region);
		int oldId = tile.id, oldSubId = tile.subId;

		tile.subId = subId;

		if (region.presented) {
			foreach (GridListener listener in gridListeners) {
				listener.OnSetId (x, y, tile, oldId, oldSubId);
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

	}

	void _Set(Tile tile, int x, int y, int id, int subid, float state1, float state2, float state3) {
		TileInfo info = atlas.GetTile (id);

		/*// CHECK IF THERE IS AN OVERLAP WITH THE ARE OF ANOTHER MULTI SQUARE TILE
		*/

		// REMOVE PREVIOUS TILE BEHAVIOUR

		// CREATE TILE GO AND SET OTHER TILES
		/*if (info.tileGO != null) {
			GameObject tileGO = Instantiate (info.tileGO);

			tileGO.transform.SetParent (tileGOContainer.transform);
			tileGO.transform.localPosition = new Vector2 (x + 0.5f, y + 0.5f);

			TileBehaviour behaviour = tileGO.GetComponent<TileBehaviour> ();

			if (behaviour == null) { 
				tileGOs.Set (x, y, tileGO.transform);
			} else {
				tileGOs.Set (x, y, behaviour);
			}

			bool[,] area = behaviour.area;
			for (int i = 0; i < area.GetLength (0); i++) {
				for (int j = 0; j < area.GetLength (1); j++) {
					if (i != behaviour.areaBottomLeftXOffset || j != behaviour.areaBottomLeftYOffset) {
						Tile otherTile = GetOrCreate (behaviour.areaBottomLeftXOffset + x + i, behaviour.areaBottomLeftYOffset + y + j);

						otherTile.id = -1;
						otherTile.shape = TileShape.EMPTY;
					}
				}
			}
		}*/

		tile.id = id;
		if (subid != int.MinValue) tile.subId = subid;

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