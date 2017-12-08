using UnityEngine;
using System.Collections;

namespace Gridlike {

	/// <summary>
	/// A utility class that contains useful grid related algorithms.
	/// </summary>
	public static class GridUtility {

		/// <summary>
		/// Gets the closest non empty tile in the grid. Of none are found, will return a random position of a tile around the specified location.
		/// </summary>
		/// <param name="grid">Grid.</param>
		/// <param name="position">The tile next to which the empty tile must be located.</param>
		/// <param name="x">The x coordinate of the non empty tile.</param>
		/// <param name="y">The y coordinate of the non empty tile.</param>
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

		/// <summary>
		/// Finds a grid with a non empty tile under the provided position. If the function fails, grid is null.
		/// </summary>
		/// <param name="position">The position under which the tile is to be found.</param>
		/// <param name="grid">The found grid.</param>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
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
		/// <summary>
		/// Finds a grid with an empty tile under the provided postion next to a non empty tile. If the function fails, the grid is null.
		/// </summary>
		/// <param name="position">The position under which the tile is to be found.</param>
		/// <param name="grid">The found grid.</param>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
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
		/// <summary>
		/// Finds a grid with no tile in the area specified by the position, width and height. If the function fails, grid is null.
		/// </summary>
		/// <param name="position">The bottom left corner of the area in world space.</param>
		/// <param name="width">The width in tile space of the area.</param>
		/// <param name="height">The height in tile space of the area.</param>
		/// <param name="grid">The grid found.</param>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
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