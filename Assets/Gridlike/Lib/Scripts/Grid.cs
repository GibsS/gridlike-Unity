using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	[SerializeField]
	List<GridListener> gridListeners;

	public const int REGION_SIZE = 50;

	int _tileSize = 1;

	[SerializeField]
	public InfiniteGrid tiles;

	public int tileSize {
		get { return _tileSize; }
		set { if(value != _tileSize) SetTileSize (value); }
	}

	void Init() {
		if (tiles == null) {
			tiles = new InfiniteGrid (REGION_SIZE);

			gridListeners = new List<GridListener> ();

			foreach (GridListener listener in GetComponents<GridListener> ()) {
				listener.ResetGrid ();
			}
		}
	}
	void Reset() {
		Init ();
	}
	void Awake() {
		Init ();
	}

	void OnDestroy() {
		foreach (GridListener listener in GetComponents<GridListener> ()) {
			listener.ResetGrid ();
		}
	}

	public void AddListener(GridListener listener) {
		gridListeners.Add (listener);
	}
	public void RemoveListener(GridListener listener) {
		gridListeners.Remove (listener);
	}

	public Tile Get(int x, int y) {
		return tiles.Get (x, y);
	}
	public TileShape GetShape(int x, int y) {
		Tile tile = tiles.Get (x, y);

		return tile == null ? TileShape.EMPTY : tile.shape;
	}
	public int GetId(int x, int y) {
		Tile tile = tiles.Get (x, y);

		return tile == null ? 0 : tile.id;
	}
	public int GetSubId(int x, int y) {
		Tile tile = tiles.Get (x, y);

		return tile == null ? 0 : tile.subId;
	}

	public void Set(int x, int y, TileShape shape, int id, int subid, int state1, int state2, int state3) {
		Tile tile = GetOrCreate (x, y);

		tile.shape = shape;

		tile.id = id;
		tile.subId = subid;

		tile.state1 = state1;
		tile.state2 = state2;
		tile.state3 = state3;

		foreach (GridListener listener in gridListeners) {
			listener.OnTileChange (x, y);
		}
	}
	public void SetShape(int x, int y, TileShape shape) {
		GetOrCreate (x, y).shape = shape;
	}
	public void SetId(int x, int y, int id) {
		GetOrCreate (x, y).id = id;
	}
	public void SetSubId(int x, int y, int subId) {
		GetOrCreate (x, y).subId = subId;
	}
		
	Tile GetOrCreate(int x, int y) {
		Tile tile = tiles.Get (x, y);

		if (tile == null) {
			tile = new Tile ();

			tiles.Set (x, y, tile);
		}

		return tile;
	}

	public void SetTileSize(int tileSize) {
		this._tileSize = tileSize;
	}

	#region REFERENTIAL

	public Vector2 TileCenterInWorld(int x, int y) {
		return transform.TransformPoint (TileCenterInTransform (x, y));
	}
	public Vector2 TileCenterInTransform(int x, int y) {
		return new Vector2 ((x + 0.5f) * _tileSize, (y + 0.5f) * _tileSize); 
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