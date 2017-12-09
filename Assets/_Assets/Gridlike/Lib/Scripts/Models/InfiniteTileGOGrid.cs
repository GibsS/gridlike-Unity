using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Wrapper to the InfiniteComponentGrid that handles the invariant's relative to tileGOs (they can't overlap one another).
	/// </summary>
	[System.Serializable]
	public class InfiniteTileGOGrid {

		/// <summary>
		/// The InfiniteComponenGrid storing every tileGO. If a tileGO has a TileBehaviour, a reference for this tilebehaviour can be found on 
		/// every tile it covers in componentGrid. If a tileGO has no TileBehaviour, it just occupies its center.
		/// </summary>
		[SerializeField] InfiniteComponentGrid componentGrid;
		/// <summary>
		/// The GO that contains every tileGO.
		/// </summary>
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

		/// <summary>
		/// Determines whether there is a tileGO at the specified position.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public bool HasTileGO(int x, int y) {
			return componentGrid.Get (x, y) != null;
		}
		/// <summary>
		/// Gets the tile behaviour at the specified position.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public TileBehaviour GetTileBehaviour(int x, int y) {
			Component component = componentGrid.Get (x, y);

			if (component is TileBehaviour) {
				return component as TileBehaviour;
			} else {
				return null;
			}
		}
		/// <summary>
		/// Gets the component at the specified position.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public Component GetComponent(int x, int y) {
			return componentGrid.Get (x, y);
		}
		/// <summary>
		/// Gets the actual GO of the tileGO at the specified position.
		/// </summary>
		/// <returns>The tile G.</returns>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public GameObject GetTileGO(int x, int y) {
			Component component = componentGrid.Get (x, y);

			if (component != null) {
				return component.gameObject;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Checks if the specified TileBehaviour would overlap any other tileGO if placed at the specified location.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
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

		/// <summary>
		/// Tries to create the tileGO associated to the provided info.
		/// </summary>
		/// <returns>The created tileGO component (TileBehaviour if the tileGO has a TileBehaviour, Transform of the tileGO if not)</returns>
		/// <param name="info">The tile type.</param>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		/// <param name="callback">The callback that is called for every affected tile (tiles covered by the new tileGO).</param>
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

		/// <summary>
		/// Removes and destroys the tileGO at the given location.
		/// </summary>
		/// <returns>The component of the tileGO (TileBehaviour or Transform, first found)</returns>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		/// <param name="callback">The callback that is called for every affected tile (tiles covered by the new tileGO).</param>
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

		/// <summary>
		/// Wrapper for the underlying InfiniteComponentGrid GetRegion method.
		/// </summary>
		/// <returns>The region.</returns>
		/// <param name="regionX">The X region coordinate.</param>
		/// <param name="regionY">The Y region coordinate.</param>
		public FiniteComponentGrid GetRegion(int regionX, int regionY) {
			return componentGrid.GetRegion (regionX, regionY);
		}
	}
}