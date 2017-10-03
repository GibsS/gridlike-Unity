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
				BoxCollider2D rect = wrapper.box as BoxCollider2D;

				if (wrapper.width == 1) {
					if (wrapper.height == 1) {
						DestroyImmediate (wrapper.gameObject);
					} else {
						if (wrapper.bottomLeftY == y) {
							wrapper.height -= 1;
							wrapper.bottomLeftY += 1;

							rect.size = new Vector2(rect.size.x, rect.size.y - 1);
							rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x, rect.transform.localPosition.y + 1);
						} else {
							wrapper.height -= 1;
							wrapper.bottomLeftY -= 1;

							rect.size = new Vector2(rect.size.x, rect.size.y - 1);
							rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x, rect.transform.localPosition.y - 1);

							int endY = wrapper.bottomLeftY + wrapper.height - 1;
							if (endY != y) {
								GridColliderPart part = GridColliderPart.BoxGridColliderPart (gameObject, grid, x, y + 1, 1, endY - y);

								for (int i = y + 1; i <= endY; i++) {
									components.Set (x, i, part);
								}
							}
						}
					}
				} else {
					if (wrapper.bottomLeftX == x) {
						wrapper.width -= 1;
						wrapper.bottomLeftX += 1;

						rect.size = new Vector2(rect.size.x - 1, rect.size.y);
						rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x + 0.5f, rect.transform.localPosition.y);
					} else {
						int endX = wrapper.bottomLeftX + wrapper.width - 1;

						wrapper.width = x - wrapper.bottomLeftX;

						rect.size = new Vector2(wrapper.width, rect.size.y);
						rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x - (endX - x + 1f)/2f, rect.transform.localPosition.y);

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
		if (tile.shape == TileShape.FULL) {
			GridColliderPart left = components.Get (x - 1, y) as GridColliderPart;

			bool expanded = false;
			if (left != null && left.height == 1 && left.box != null) {
				left.width += 1;

				BoxCollider2D box = left.box as BoxCollider2D;
				box.transform.localPosition = new Vector2 (box.transform.localPosition.x + 0.5f, box.transform.localPosition.y);
				box.size = new Vector2 (box.size.x + 1, 1);

				components.Set (x, y, left);

				expanded = true;
			}
				
			GridColliderPart right = components.Get (x + 1, y) as GridColliderPart;
			if (right != null && right.height == 1 && right.box != null) {
				if (left == null) {
					right.width += 1;
					right.bottomLeftX -= 1;

					BoxCollider2D box = right.box as BoxCollider2D;
					box.transform.localPosition = new Vector2 (box.transform.localPosition.x - 0.5f, box.transform.localPosition.y);
					box.size = new Vector2 (box.size.x + 1, 1);

					components.Set (x, y, right);

					expanded = true;
				} else {
					left.width += right.width;
					left.height = 1;

					BoxCollider2D box = left.box as BoxCollider2D;

					box.transform.localPosition = new Vector2 (box.transform.localPosition.x + right.width/2f, box.transform.localPosition.y);
					box.size = new Vector2 (box.size.x + right.width, 1);

					for (int i = right.bottomLeftX; i < right.bottomLeftX + right.width; i++) {
						components.Set (i, y, left);
					}

					DestroyImmediate (right.gameObject);

					expanded = true;
				}
			}

			if (!expanded) {
				left = GridColliderPart.BoxGridColliderPart (gameObject, grid, x, y, 1, 1);

				components.Set (x, y, left);
			}
		}
	}
	public override void OnRegionChange(int regionX, int regionY) {

	}

	public override void OnTileSizeChange() {

	}
}