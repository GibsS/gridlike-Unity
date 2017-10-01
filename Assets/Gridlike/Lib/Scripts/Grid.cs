using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public const int REGION_SIZE = 50;

	int _tileSize = 1;

	[SerializeField]
	public InfiniteGrid tiles;

	public int tileSize {
		get { return _tileSize; }
		set { if(value != _tileSize) SetTileSize (value); }
	}

	void Reset() {
		tiles = new InfiniteGrid (REGION_SIZE);
	}

	public void SetShape(int x, int y, TileShape shape) {
		Tile tile = tiles.Get (x, y);

		if (tile == null) {
			tile = new Tile ();

			tiles.Set (x, y, tile);
		}

		tile.shape = shape;
	}

	public void SetTileSize(int tileSize) {
		this._tileSize = tileSize;
	}

	public void WorldToGrid(Vector2 position, out int x, out int y) {
		position = transform.InverseTransformPoint (position);

		TransformToGrid (position, out x, out y);
	}

	public void TransformToGrid(Vector2 position, out int x, out int y) {
		x = Mathf.FloorToInt(((float)position.x) / tileSize);
		y = Mathf.FloorToInt(((float)position.y) / tileSize);
	}
}