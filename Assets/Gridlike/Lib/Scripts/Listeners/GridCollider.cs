using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCollider : GridListener {

	[SerializeField]
	InfiniteComponentGrid components;

	public override void ResetGrid() {
		base.ResetGrid ();

		if (components == null) {
			components = new InfiniteComponentGrid (Grid.REGION_SIZE);
		}
	}

	public override void OnSet(int x, int y, Tile tile) {
		GridColliderPart wrapper = components.Get (x, y) as GridColliderPart;

		// clear previous
		if (wrapper != null) {
			if (wrapper.box != null) {
				
				if (wrapper.width == 1) {
					if (wrapper.height == 1) {
						DestroyImmediate (wrapper.gameObject);
					} else {
						if (wrapper.bottomLeftY == y) {
							wrapper.bottomLeftY += 1;

							wrapper.SetSize(wrapper.width, wrapper.height - 1);
							wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x, wrapper.transform.localPosition.y + 1);
						} else {
							wrapper.bottomLeftY -= 1;

							wrapper.SetSize(wrapper.width, wrapper.height - 1);
							wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x, wrapper.transform.localPosition.y - 1);

							int endY = wrapper.bottomLeftY + wrapper.height - 1;
							if (endY != y) {
								GridColliderPart part = GridColliderPart.CreateColliderPart (gameObject, grid, wrapper.shape, x, y + 1, 1, endY - y);

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
							GridColliderPart part = GridColliderPart.BoxGridColliderPart (gameObject, grid, x + 1, y, endX - x, 1);

							for (int i = x + 1; i <= endX; i++) {
								components.Set (i, y, part);
							}
						}
					}
				}
			}
		}

		// add new
		if (tile.shape != TileShape.EMPTY) {
			bool expanded = false;

			GridColliderPart left = components.Get (x - 1, y) as GridColliderPart;
			if (left != null && left.height == 1 && left.Compatible(tile)) {
				left.transform.localPosition = new Vector2 (left.transform.localPosition.x + 0.5f, left.transform.localPosition.y);
				left.SetSize (left.width + 1, 1);

				components.Set (x, y, left);

				expanded = true;
			}
				
			GridColliderPart right = components.Get (x + 1, y) as GridColliderPart;
			if (right != null && right.height == 1 && right.Compatible(tile)) {
				if (left == null) {
					right.bottomLeftX -= 1;

					right.transform.localPosition = new Vector2 (right.transform.localPosition.x - 0.5f, right.transform.localPosition.y);
					right.SetSize (right.width + 1, 1);

					components.Set (x, y, right);

					expanded = true;
				} else {
					left.transform.localPosition = new Vector2 (left.transform.localPosition.x + right.width/2f, left.transform.localPosition.y);
					left.SetSize (left.width + right.width, 1);

					for (int i = right.bottomLeftX; i < right.bottomLeftX + right.width; i++) {
						components.Set (i, y, left);
					}

					DestroyImmediate (right.gameObject);

					expanded = true;
				}
			}

			if (!expanded && tile.shape != TileShape.FULL) {
				Debug.Log ("Try and expand a " + tile.shape);
				GridColliderPart down = components.Get (x, y - 1) as GridColliderPart;
				if (down != null && down.width == 1 && down.Compatible (tile)) {
					down.transform.localPosition = new Vector2 (down.transform.localPosition.x, down.transform.localPosition.y + 0.5f);
					down.SetSize (1, down.height + 1);

					components.Set (x, y, down);

					expanded = true;
				}

				GridColliderPart up = components.Get (x + 1, y) as GridColliderPart;
				if (up != null && up.width == 1 && up.Compatible (tile)) {
					if (up == null) {
						up.bottomLeftY -= 1;

						up.transform.localPosition = new Vector2 (up.transform.localPosition.x, up.transform.localPosition.y - 0.5f);
						up.SetSize (right.width + 1, 1);

						components.Set (x, y, up);

						expanded = true;
					} else {
						down.transform.localPosition = new Vector2 (down.transform.localPosition.x + down.height / 2f, down.transform.localPosition.y);
						down.SetSize (1, down.height + up.height);

						for (int i = up.bottomLeftY; i < up.bottomLeftY + up.width; i++) {
							components.Set (i, y, down);
						}

						DestroyImmediate (up.gameObject);

						expanded = true;
					}
				}
			}

			if(!expanded) {
				Debug.Log ("Adding a " + tile.shape);
				left = GridColliderPart.CreateColliderPart (gameObject, grid, tile.shape, x, y, 1, 1);

				components.Set (x, y, left);
			}
		}
	}
	public override void OnRegionChange(int regionX, int regionY) {

	}

	public override void OnTileSizeChange() {

	}
}