using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InfiniteTileGrid : InfiniteGrid<Tile> { 
	
	public InfiniteTileGrid(int gridSize) : base(gridSize) { }
}

public class Grid : MonoBehaviour {

	public const int REGION_SIZE = 50;

	public int tileSize = 1;

	[SerializeField]
	public InfiniteTileGrid tiles;

	void Reset() {
		tiles = new InfiniteTileGrid (REGION_SIZE);
	}

	public void SetShape(int x, int y, TileShape shape) {
		Tile tile = tiles.Get (x, y);

		if (tile == null) {
			tile = new Tile ();

			tiles.Set (x, y, tile);
		}

		tile.shape = shape;
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