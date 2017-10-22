using UnityEngine;

// TODO move
public delegate void PositionCallback(int x, int y);

[System.Serializable]
public class InfiniteTileGOGrid {

	[SerializeField] InfiniteComponentGrid componentGrid;
	[SerializeField] GameObject container;

	public InfiniteTileGOGrid(GameObject parent, int regionSize) {
		componentGrid = new InfiniteComponentGrid (regionSize);

		foreach (Transform t in parent.transform) {
			if (t.name == "tile container") {
				UnityEngine.Object.DestroyImmediate (t.gameObject);
			}
		}

		container = new GameObject ("tile container");
		container.transform.SetParent (parent.transform, false);
	}

	public bool HasTileGO(int x, int y) {
		return componentGrid.Get (x, y) != null;
	}
	public TileBehaviour GetTileBehaviour(int x, int y) {
		Component component = componentGrid.Get (x, y);

		if (component is TileBehaviour) {
			return component as TileBehaviour;
		} else {
			return null;
		}
	}
	public Component GetComponent(int x, int y) {
		return componentGrid.Get (x, y);
	}
	public GameObject GetTileGO(int x, int y) {
		Component component = componentGrid.Get (x, y);

		if (component != null) {
			return component.gameObject;
		} else {
			return null;
		}
	}

	public bool HasOverlap(int x, int y, TileBehaviour behaviour) {
		x += behaviour.areaBottomLeftXOffset;
		y += behaviour.areaBottomLeftYOffset;

		bool[,] area = behaviour.area;

		for (int i = 0; i < area.GetLength (0); i++) {
			for (int j = 0; j < area.GetLength (1); j++) {
				if (area [i, j] && GetTileGO (x + i, y + j) != null) {
					return true;
				}
			}
		}

		return false;
	}

	// true if succeeds or there is no GO
	public bool TryCreateTileGO(TileInfo info, int x, int y, PositionCallback callback) {
		if (info.tileGO != null) {
			TileBehaviour behaviour = info.tileGO.GetComponent<TileBehaviour> ();

			if (behaviour == null) {
				if (componentGrid.Get (x, y) != null) {
					return false;
				}
			} else {
				if (HasOverlap (x, y, behaviour)) {
					return false;
				}
			}

			_CreateTileGO (info.tileGO, x, y, callback);
		}

		return true;
	}

	// PRECONDITION: space is available!
	public void CreateTileGO(TileInfo info, int x, int y, PositionCallback callback) {
		if (info.tileGO != null) {
			_CreateTileGO (info.tileGO, x, y, callback);
		}
	}
	void _CreateTileGO(GameObject prefab, int x, int y, PositionCallback callback) {
		GameObject tile = Object.Instantiate (prefab);
		tile.transform.SetParent (container.transform);
		tile.transform.localPosition = new Vector2 (x + 0.5f, y + 0.5f);

		TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour> ();

		if (tileBehaviour != null) {
			tileBehaviour._x = x;
			tileBehaviour._y = y;

			x += tileBehaviour.areaBottomLeftXOffset;
			y += tileBehaviour.areaBottomLeftYOffset;

			bool[,] area = tileBehaviour.area;

			for (int i = 0; i < area.GetLength (0); i++) {
				for (int j = 0; j < area.GetLength (1); j++) {
					if (area [i, j]) {
						componentGrid.Set (x + i, y + j, tileBehaviour);

						callback (x + i, y + j);
					}
				}
			}

			componentGrid.Set (
				tileBehaviour._x, 
				tileBehaviour._y, 
				tileBehaviour
			);
		} else {
			componentGrid.Set (x, y, tile.transform);
		}
	}

	public void DestroyTileGO(int x, int y, PositionCallback callback) {
		Component component = componentGrid.Get (x, y);

		if (component != null) {
			if (component is TileBehaviour) {
				x = (component as TileBehaviour)._x + (component as TileBehaviour).areaBottomLeftXOffset;
				y = (component as TileBehaviour)._y + (component as TileBehaviour).areaBottomLeftYOffset;

				bool[,] area = (component as TileBehaviour).area;

				for (int i = 0; i < area.GetLength (0); i++) {
					for (int j = 0; j < area.GetLength (1); j++) {
						if (area [i, j]) {
							componentGrid.Set (x + i, y + j, null);

							callback (x + i, y + j);
						}
					}
				}
			}

			componentGrid.Set ((component as TileBehaviour)._x, (component as TileBehaviour)._y, null);
			Object.DestroyImmediate (component.gameObject);
		}
	}

	public FiniteComponentGrid GetRegion(int regionX, int regionY) {
		return componentGrid.GetRegion (regionX, regionY);
	}
}