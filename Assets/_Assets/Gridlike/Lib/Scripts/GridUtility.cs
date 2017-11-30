using UnityEngine;
using System.Collections;

namespace Gridlike {

	public static class GridUtility {
		
		public static void GetClosestNonEmptyTile(Grid grid, Vector2 position, out int x, out int y) {
			grid.WorldToGrid (position, out x, out y);

			if (grid.GetId (x, y) != 0 || grid.GetTileComponent(x, y) != null) return;
			if (grid.GetId (x + 1, y) != 0 || grid.GetTileComponent(x + 1, y) != null) {
				x++;
				return;
			}
			if (grid.GetId (x, y + 1) != 0 || grid.GetTileComponent(x, y + 1) != null) {
				y++;
				return;
			}
			if (grid.GetId (x - 1, y) != 0 || grid.GetTileComponent(x - 1, y) != null) {
				x--;
				return;
			}
			if (grid.GetId (x, y - 1) != 0 || grid.GetTileComponent(x, y - 1) != null) {
				y--;
				return;
			}
			if (grid.GetId (x + 1, y + 1) != 0 || grid.GetTileComponent(x + 1, y + 1) != null) {
				x++;
				y++;
				return;
			}
			if (grid.GetId (x + 1, y - 1) != 0 || grid.GetTileComponent(x + 1, y - 1) != null) {
				x++;
				y--;
				return;
			}
			if (grid.GetId (x - 1, y - 1) != 0 || grid.GetTileComponent(x - 1, y - 1) != null) {
				x--;
				y--;
				return;
			}
			if (grid.GetId (x - 1, y + 1) != 0 || grid.GetTileComponent(x - 1, y + 1) != null) {
				x--;
				y++;
				return;
			}
		}

		public static void GetAnyNonEmptyTile(Vector2 position, out Grid grid, out int x, out int y) {
			grid = null;
			x = 0;
			y = 0;

			foreach(Grid candidate in Grid.GetAllGrids()) {
				candidate.WorldToGrid (position, out x, out y);

				if (candidate.GetId (x, y) != 0 || candidate.GetTileComponent(x, y) != null) {
					grid = candidate;
					return;
				}
			}
		}
		public static void GetEmptyNextToBlock(Vector2 position, out Grid grid, out int x, out int y) {
			grid = null;
			x = 0;
			y = 0;

			foreach(Grid candidate in Grid.GetAllGrids()) {
				candidate.WorldToGrid (position, out x, out y);

				if (candidate.GetId (x, y) == 0 
					&& candidate.GetTileComponent(x, y) == null
					&& (candidate.GetId(x - 1, y) != 0 
						|| candidate.GetId(x + 1, y) != 0
						|| candidate.GetId(x, y - 1) != 0
						|| candidate.GetId(x, y + 1) != 0)) {

					grid = candidate;
					return;
				}
			}
		}
		public static void GetEmptyInAreaOverBlock(Vector2 position, int width, int height, out Grid grid, out int x, out int y) {
			grid = null;
			x = 0;
			y = 0;

			foreach (Grid candidate in Grid.GetAllGrids()) {
				candidate.WorldToGrid (position, out x, out y);

				bool fail = false;

				for (int i = x; i < x + width && !fail; i++) {
					for (int j = y; j < y + height; j++) {
						Tile tile = candidate.Get (i, j);

						if ((tile != null && tile.id != 0) || candidate.GetTileComponent (i, j) != null) {
							fail = true;
							break;
						}
					}
				}

				for (int i = x; i < x + width; i++) {
					Tile tile = candidate.Get (i, y - 1);

					if (tile == null || tile.id == 0) {
						fail = true;
						break;
					}
				}

				if (!fail) {
					grid = candidate;
					break;
				}
			}
		}
	}
}