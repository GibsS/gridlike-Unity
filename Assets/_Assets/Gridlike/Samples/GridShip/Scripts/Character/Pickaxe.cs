using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class Pickaxe : Tool {

	public float cooldown;
	public float radius;

	float lastPickaxe = -10000;

	public override void OnMouseAny (Vector2 position) {
		TryPickaxe (position);
	}

	void TryPickaxe(Vector2 position) {
		if(Time.time - lastPickaxe > cooldown && Vector2.Distance((Vector2)transform.position, position) < radius) {
			lastPickaxe = Time.time;

			Grid grid;
			int x;
			int y;

			GridUtility.GetAnyNonEmptyTile (position, out grid, out x, out y);

			if (grid != null) grid.Clear (x, y);
		}
	}
}
