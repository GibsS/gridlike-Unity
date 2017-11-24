using UnityEngine;
using System.Collections;

using Gridlike;

public class GridUtility {

	public static void GetClosestNonEmptyTile(Grid grid, Vector2 position, out int x, out int y) {
		grid.WorldToGrid (position, out x, out y);

		if (grid.GetId (x, y) != 0) return;
		if (grid.GetId (x + 1, y) != 0) {
			x++;
			return;
		}
		if (grid.GetId (x, y + 1) != 0) {
			y++;
			return;
		}
		if (grid.GetId (x - 1, y) != 0) {
			x--;
			return;
		}
		if (grid.GetId (x, y - 1) != 0) {
			y--;
			return;
		}
		if (grid.GetId (x + 1, y + 1) != 0) {
			x++;
			y++;
			return;
		}
		if (grid.GetId (x + 1, y - 1) != 0) {
			x++;
			y--;
			return;
		}
		if (grid.GetId (x - 1, y - 1) != 0) {
			x--;
			y--;
			return;
		}
		if (grid.GetId (x - 1, y + 1) != 0) {
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

			if (candidate.GetId (x, y) != 0) {
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
				&& (candidate.GetId(x - 1, y) != 0 
				|| candidate.GetId(x + 1, y) != 0
				|| candidate.GetId(x, y - 1) != 0
				|| candidate.GetId(x, y + 1) != 0)) {
				
				grid = candidate;
				return;
			}
		}
	}

	public static void ExplodeInAllGrid(GSCharacter character, Vector2 position, int radius, int damage) {
		foreach (Grid grid in Grid.GetAllGrids()) {
			GSGrid wrapper = grid.GetComponent<GSGrid> ();

			if (wrapper != null) {
				wrapper.Explosion (character, position, radius, damage);
			} else {
				Debug.LogWarning ("Grid has no GSGrid");
			}
		}
	}
}