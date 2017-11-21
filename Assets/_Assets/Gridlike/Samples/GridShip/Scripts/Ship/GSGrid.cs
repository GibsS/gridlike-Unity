using UnityEngine;
using System.Collections;

using Gridlike;

public class GSGrid : MonoBehaviour {

	Grid grid;

	void Start() {
		grid = GetComponent<Grid>();
	}

	public void Damage(GSCharacter character, int x, int y, int damage, Vector2 position) {
		Tile tile = grid.Get (x, y);
		int id = tile.id;
		int HP = (int) tile.state1;

		if (GSConsts.TileExists (id)) {
			int hpLost;

			if (HP + damage >= GSConsts.tiles [id].HP) {
				hpLost = Mathf.Min(damage, GSConsts.tiles [id].HP - HP);
				grid.Clear (x, y);
			} else {
				hpLost = damage;
				grid.SetState (x, y, HP + damage, 0, 0);
			}

			if (position == Vector2.zero) {
				position = grid.TileCenterInWorld (x, y);
			}

			CubeParticle.CreateParticles (position, character, Mathf.CeilToInt(hpLost * GSConsts.tiles[id].cubePerHP));
		}
	}
	public void Place(int x, int y, int id) {
		grid.SetId (x, y, id);
		grid.SetState (x, y, 0, 0, 0);
	}
}

