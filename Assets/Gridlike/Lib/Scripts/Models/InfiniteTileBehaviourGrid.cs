using UnityEngine;

[System.Serializable]
public class InfiniteTileBehaviourGrid {

	[SerializeField] InfiniteComponentGrid componentGrid;
	[SerializeField] GameObject container;

	public InfiniteTileBehaviourGrid(GameObject parent, int regionSize) {
		componentGrid = new InfiniteComponentGrid (regionSize);

		foreach (Transform t in parent.transform) {
			if (t.name == "tile container") {
				UnityEngine.Object.DestroyImmediate (t.gameObject);
			}
		}

		if (container == null) {
			container = new GameObject ("tile container");
			container.transform.SetParent (parent.transform, false);
		}
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
	public GameObject GetTileGO(int x, int y) {
		Component component = componentGrid.Get (x, y);

		if (component != null) {
			return component.gameObject;
		} else {
			return null;
		}
	}

	public void CreateTileGO(TileInfo info, int x, int y) {
		if (info.tileGO != null) {
			GameObject tile = Object.Instantiate (info.tileGO);
			tile.transform.SetParent (container.transform);
			tile.transform.localPosition = new Vector2 (x + 0.5f, y + 0.5f);

			TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour> ();

			if (tileBehaviour != null) {
				x += tileBehaviour.areaBottomLeftXOffset;
				y += tileBehaviour.areaBottomLeftYOffset;

				bool[,] area = tileBehaviour.area;

				for (int i = 0; i < area.GetLength (0); i++) {
					for (int j = 0; j < area.GetLength (1); j++) {
						if (area [i, j]) {
							componentGrid.Set (x + i, y + j, tileBehaviour);
						}
					}
				}

				componentGrid.Set (x, y, tileBehaviour);
			} else {
				componentGrid.Set (x, y, tile.transform);
			}
		}
	}

	public void DestroyTileGO(int x, int y) {
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
						}
					}
				}
			}

			componentGrid.Set (x, y, null);
			Object.DestroyImmediate (component.gameObject);
		}
	}

	public FiniteComponentGrid GetRegion(int regionX, int regionY) {
		return componentGrid.GetRegion (regionX, regionY);
	}
}