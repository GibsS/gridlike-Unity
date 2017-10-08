using System;
using System.Collections.Generic;
using UnityEngine;

// TODO destroy leads to destroy colliders
public class GridCollider : GridListener {

	[HideInInspector]
	[SerializeField]
	InfiniteComponentGrid components;

	[SerializeField] GameObject containerGO;

	public override void OnDestroy() {
		base.OnDestroy ();

		DestroyImmediate (containerGO);
	}

	public override void ResetGrid() {
		base.ResetGrid ();

		if (components == null) {
			components = new InfiniteComponentGrid (Grid.REGION_SIZE);
		}

		if (containerGO == null) {
			containerGO = new GameObject ("colliders");
			containerGO.transform.SetParent (transform);
		}
	}

	public override void OnSet(int x, int y, Tile tile) {
		GridColliderPart wrapper = components.Get (x, y) as GridColliderPart;

		// clear previous
		if (wrapper != null) {
			if (wrapper.width == 1) {
				if (wrapper.height == 1) {
					DestroyImmediate (wrapper.gameObject);
				} else {
					if (wrapper.bottomLeftY == y) {
						wrapper.bottomLeftY += 1;

						wrapper.SetSize(wrapper.width, wrapper.height - 1);
						wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x, wrapper.transform.localPosition.y + 0.5f);
					} else {
						int endY = wrapper.bottomLeftY + wrapper.height - 1;

						wrapper.SetSize(wrapper.width, y - wrapper.bottomLeftY);
						wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x, wrapper.transform.localPosition.y - (endY - y + 1f)/2f);

						if (endY != y) {
							GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, wrapper.shape, x, y + 1, 1, endY - y);

							for (int i = y + 1; i <= endY; i++) {
								components.Set (x, i, part);
							}
						}
					}
				}
			} else {
				if (wrapper.bottomLeftX == x) {
					wrapper.bottomLeftX += 1;

					wrapper.SetSize (wrapper.width - 1, wrapper.height);
					wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x + 0.5f, wrapper.transform.localPosition.y);
				} else {
					int endX = wrapper.bottomLeftX + wrapper.width - 1;

					wrapper.SetSize (x - wrapper.bottomLeftX, wrapper.height);
					wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x - (endX - x + 1f)/2f, wrapper.transform.localPosition.y);

					if (endX != x) {
						GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, wrapper.shape, x + 1, y, endX - x, 1);

						for (int i = x + 1; i <= endX; i++) {
							components.Set (i, y, part);
						}
					}
				}
			}
		}

		components.Set (x, y, null);

		// add new
		if (tile.shape != TileShape.EMPTY) {
			// HORIZONTAL GROWTH
			if (tile.shape != TileShape.LEFT_ONEWAY && tile.shape != TileShape.RIGHT_ONEWAY) {
				bool expanded = false;

				GridColliderPart left = components.Get (x - 1, y) as GridColliderPart;
				if (left != null && left.height == 1 && left.Compatible (tile)) {
					left.transform.localPosition = new Vector2 (left.transform.localPosition.x + 0.5f, left.transform.localPosition.y);
					left.SetSize (left.width + 1, 1);

					components.Set (x, y, left);

					expanded = true;
				}
				
				GridColliderPart right = components.Get (x + 1, y) as GridColliderPart;
				if (right != null && right.height == 1 && right.Compatible (tile)) {
					if (!expanded) {
						right.bottomLeftX -= 1;

						right.transform.localPosition = new Vector2 (right.transform.localPosition.x - 0.5f, right.transform.localPosition.y);
						right.SetSize (right.width + 1, 1);

						components.Set (x, y, right);

						return;
					} else {
						left.transform.localPosition = new Vector2 (left.transform.localPosition.x + right.width / 2f, left.transform.localPosition.y);
						left.SetSize (left.width + right.width, 1);

						for (int i = right.bottomLeftX; i < right.bottomLeftX + right.width; i++) {
							components.Set (i, y, left);
						}

						DestroyImmediate (right.gameObject);

						return;
					}
				}

				if (expanded) return;
			}

			// VERTICAL GROWTH
			if (tile.shape != TileShape.FULL && tile.shape != TileShape.UP_ONEWAY && tile.shape != TileShape.DOWN_ONEWAY) {
				bool expanded = false;

				GridColliderPart down = components.Get (x, y - 1) as GridColliderPart;
				if (down != null && down.width == 1 && down.Compatible (tile)) {
					down.transform.localPosition = new Vector2 (down.transform.localPosition.x, down.transform.localPosition.y + 0.5f);
					down.SetSize (1, down.height + 1);

					components.Set (x, y, down);

					expanded = true;
				}

				GridColliderPart up = components.Get (x, y + 1) as GridColliderPart;
				if (up != null && up.width == 1 && up.Compatible (tile)) {
					if (!expanded) {
						up.bottomLeftY -= 1;

						up.transform.localPosition = new Vector2 (up.transform.localPosition.x, up.transform.localPosition.y - 0.5f);
						up.SetSize (1, up.height + 1);

						components.Set (x, y, up);

						return;
					} else {
						down.transform.localPosition = new Vector2 (down.transform.localPosition.x, down.transform.localPosition.y + up.height / 2f);
						down.SetSize (1, down.height + up.height);

						for (int i = up.bottomLeftY; i < up.bottomLeftY + up.width; i++) {
							components.Set (i, y, down);
						}

						DestroyImmediate (up.gameObject);

						return;
					}
				}

				if (expanded) return;
			}

			// NO EXPANSE, CREATE NEW
			components.Set (x, y, GridColliderPart.CreateColliderPart (containerGO, grid, tile.shape, x, y, 1, 1));
		}
	}

	public override void OnHideRegion() {
		Debug.Log ("[GridCollider.OnHideRegion] NOT IMPLEMENTED");
	}

	public override void OnTileSizeChange() {
		Debug.Log ("[GridCollider.OnTileSizeChange] NOT IMPLEMENTED");
	}
}