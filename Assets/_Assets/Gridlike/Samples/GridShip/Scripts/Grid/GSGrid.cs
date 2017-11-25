using UnityEngine;
using System.Collections;

using Gridlike;

public class GSGrid : MonoBehaviour {

	Grid grid;

	void Start() {
		grid = GetComponent<Grid>();
	}

	public void Explosion(GSCharacter character, Vector2 position, int radius, int damage) {
		int x;
		int y;

		grid.WorldToGrid (position, out x, out y);

		for (int i = x - radius; i <= x + radius; i++) {
			for (int j = y - radius; j <= y + radius; j++) {
				int dx = i - x;
				int dy = j - y;

				if (dx * dx + dy * dy <= radius * radius) {
					Damage (character, i, j, damage, grid.TileCenterInWorld (i, j));
				}
			}
		}
	}

	public void Damage(GSCharacter character, int x, int y, int damage, Vector2 position) {
		Tile tile = grid.Get (x, y);

		if (tile != null) {
			GSTileBehaviour behaviour = grid.GetTileComponent (x, y) as GSTileBehaviour;
			int id = tile.id;
			int HP = (int) tile.state1;

			if (behaviour != null) {
				id = behaviour.tile.id;
				x = behaviour.x;
				y = behaviour.y;

				HP = (int) grid.Get (x, y).state1;
			}

			if (GSConsts.TileExists (id)) {
				int hpLost;

				if (HP + damage >= GSConsts.tiles [id].HP) {
					hpLost = Mathf.Min (damage, GSConsts.tiles [id].HP - HP);
					grid.Clear (x, y);
				} else {
					hpLost = damage;
					grid.SetState (x, y, HP + damage, 0, 0);
				}

				if (position == Vector2.zero) {
					position = grid.TileCenterInWorld (x, y);
				}

				CubeParticle.CreateParticles (position, character, Mathf.CeilToInt (hpLost * GSConsts.tiles [id].cubePerHP));
			} else {
				grid.Clear (x, y);
			}
		}
	}
	public bool CanPlace(int x, int y, int id) {
		return grid.CanSet (x, y, id);
	}

	public void Place(int x, int y, int id) {
		if(CanPlace(x, y, id)) {
			grid.SetId (x, y, id);
			grid.SetState (x, y, 0, 0, 0);
		}
	}
}

