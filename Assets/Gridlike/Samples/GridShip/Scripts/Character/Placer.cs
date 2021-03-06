﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

namespace Gridship {

	public class Placer : Tool {

		// TODO factor
		public TileAtlas atlas;

		public int placeId;

		public float radius;

		public override void OnMouseAny (Vector2 position) {
			TryPlace (position, placeId);
		}

		void TryPlace(Vector2 position, int id) {
			if (Vector2.Distance (transform.position, position) < radius 
				&& GSConsts.TileExists(id) 
				&& character.GetCubeCount() >= GSConsts.tiles[id].cubeCost) {

				Gridlike.Grid grid;
				int x;
				int y;

				if (atlas [id].tileGO != null) {
					GSTileBehaviour behaviour = atlas [id].tileGO.GetComponent<GSTileBehaviour> ();

					Gridlike.GridUtility.GetEmptyInAreaOverBlock (position, behaviour.width, behaviour.height, out grid, out x, out y);
				} else {
					Gridlike.GridUtility.GetEmptyNextToBlock (position, out grid, out x, out y);
				}

				if (grid != null) {
					character.ConsumeCubes(GSConsts.tiles[id].cubeCost);
					GSGrid gsGrid = grid.GetComponent<GSGrid> ();
					gsGrid.Place (x, y, id);
				}
			}
		}
	}
}