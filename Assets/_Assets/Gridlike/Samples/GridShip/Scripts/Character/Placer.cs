using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class Placer : Tool {

	public static Dictionary<int, int> costs = new Dictionary<int, int> {
		{ 1, 5 }
	};

	public int placeId;

	public float radius;

	public override void OnMouseAny (Vector2 position) {
		TryPlace (position, placeId);
	}

	void TryPlace(Vector2 position, int id) {
		if (Vector2.Distance (transform.position, position) < radius 
			&& GSConsts.TileExists(id) 
			&& character.GetCubeCount() >= GSConsts.tiles[id].cubeCost) {

			Grid grid;
			int x;
			int y;

			GridUtility.GetEmptyNextToBlock (position, out grid, out x, out y);
			if (grid != null) {
				character.ConsumeCubes(GSConsts.tiles[id].cubeCost);
				GSGrid gsGrid = grid.GetComponent<GSGrid> ();
				gsGrid.Place (x, y, id);
			}
		}
	}
}