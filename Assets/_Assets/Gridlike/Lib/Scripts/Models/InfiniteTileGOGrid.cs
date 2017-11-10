using UnityEngine;

namespace Gridlike {
	
	[System.Serializable]
	public class InfiniteTileGOGrid {

		[SerializeField] InfiniteComponentGrid componentGrid;
		[SerializeField] GameObject container;

		public InfiniteTileGOGrid(GameObject parent, int regionSize) {
			componentGrid = new InfiniteComponentGrid (regionSize);

			if (Application.isPlaying) {
				foreach (Transform t in parent.transform) {
					if (t.name == "tile container") {
						Object.Destroy (t.gameObject);
					}
				}
			} else {
				foreach (Transform t in parent.transform) {
					if (t.name == "tile container") {
						Object.DestroyImmediate (t.gameObject);
					}
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
		public Component TryCreateTileGO(TileInfo info, int x, int y, PositionCallback callback) {
			if (info.tileGO != null) {
				TileBehaviour behaviour = info.tileGO.GetComponent<TileBehaviour> ();

				if (behaviour == null) {
					if (componentGrid.Get (x, y) != null) {
						return null;
					}
				} else {
					if (HasOverlap (x, y, behaviour)) {
						return null;
					}
				}

				return _CreateTileGO (info.tileGO, x, y, callback);
			}

			return null;
		}

		// PRECONDITION: space is available!
		public Component CreateTileGO(TileInfo info, int x, int y, PositionCallback callback) {
			if (info.tileGO != null) {
				return _CreateTileGO (info.tileGO, x, y, callback);
			} else {
				return null;
			}
		}
		Component _CreateTileGO(GameObject prefab, int x, int y, PositionCallback callback) {
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

				return tileBehaviour;
			} else {
				componentGrid.Set (x, y, tile.transform);

				return tile.transform;
			}
		}

		public Component DestroyTileGO(int x, int y, PositionCallback callback) {
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
					componentGrid.Set ((component as TileBehaviour)._x, (component as TileBehaviour)._y, null);
				} else {
					componentGrid.Set (x, y, null);
				}

				if (Application.isPlaying)
					Object.Destroy (component.gameObject);
				else
					Object.DestroyImmediate (component.gameObject);

				return component;
			}
			return null;
		}

		public FiniteComponentGrid GetRegion(int regionX, int regionY) {
			return componentGrid.GetRegion (regionX, regionY);
		}
	}
}