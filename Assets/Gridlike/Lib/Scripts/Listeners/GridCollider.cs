using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Collider2DWrapper {
	
	public int x;
	public int y;

	public int width;
	public int height;

	public Collider2D collider;
}

public class GridCollider : GridListener {

	[SerializeField]
	InfiniteGrid components;

	public override void ResetGrid() {
		base.ResetGrid ();

		if (components == null) {
			components = new InfiniteGrid (Grid.REGION_SIZE);
		}
	}

	public override void OnSet(int x, int y, Tile tile) {
		/*Collider2DWrapper wrapper = components.Get (x, y) as Collider2DWrapper;

		// clear previous
		if (wrapper != null) {
			if (wrapper.collider is BoxCollider2D) {
				BoxCollider2D rect = wrapper.collider as BoxCollider2D;

				if (wrapper.width == 1) {
					if (wrapper.height == 1) {
						Destroy (rect);
					} else {
						if (wrapper.y == y) {
							wrapper.height -= 1;
							wrapper.y += 1;

							rect.size = new Vector2(rect.size.x, rect.size.y - 1);
							rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x, rect.transform.localPosition.y + 1);
						} else {
							wrapper.height -= 1;
							wrapper.y -= 1;
							//rect.size.y -= 1;
							rect.size = new Vector2(rect.size.x, rect.size.y - 1);
							rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x, rect.transform.localPosition.y - 1);

							int endY = wrapper.y + wrapper.height - 1;
							if (endY != y) {
								Collider2DWrapper newWrapper = new Collider2DWrapper();
								wrapper.x = x;
								wrapper.y = y + 1;
								wrapper.width = 1;
								wrapper.height = endY - y;

								GameObject obj = new GameObject ("box collider");

								BoxCollider2D newBox = obj.AddComponent<BoxCollider2D> ();
								newBox.size = new Vector2 (1, endY - y);
			
								obj.transform.SetParent (transform);
								obj.transform.localPosition = grid.TileCenterInTransform (x, y);
								obj.transform.localPosition += Vector3.up * (endY - y) / 2f;
								obj.transform.localScale = new Vector3 (grid.tileSize, grid.tileSize, 1);

								newWrapper.collider = newBox;

								for (int i = y + 1; i <= endY; i++) {
									components.Set (x, i, newWrapper);
								}
							}
						}
					}
				} else {
					if (wrapper.x == x) {
						wrapper.width -= 1;
						wrapper.x += 1;

						rect.size = new Vector2(rect.size.x - 1, rect.size.y);
						rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x + 1, rect.transform.localPosition.y);
					} else {
						wrapper.width -= 1;
						wrapper.x -= 1;
						//rect.size.y -= 1;
						rect.size = new Vector2(rect.size.x - 1, rect.size.y);
						rect.transform.localPosition = new Vector2 (rect.transform.localPosition.x - 1, rect.transform.localPosition.y);

						int endX = wrapper.x + wrapper.width - 1;
						if (endX != x) {
							Collider2DWrapper newWrapper = new Collider2DWrapper();
							wrapper.x = x + 1;
							wrapper.y = y;
							wrapper.width = endX - x;
							wrapper.height = 1;

							GameObject obj = new GameObject ("box collider");

							BoxCollider2D newBox = obj.AddComponent<BoxCollider2D> ();
							newBox.size = new Vector2 (1, endX - x);

							obj.transform.SetParent (transform);
							obj.transform.localPosition = grid.TileCenterInTransform (x, y);
							obj.transform.localPosition += Vector3.right * (endX - x) / 2f;
							obj.transform.localScale = new Vector3 (grid.tileSize, grid.tileSize, 1);

							newWrapper.collider = newBox;

							for (int i = x + 1; i <= endX; i++) {
								components.Set (i, y, newWrapper);
							}
						}
					}
				}
			}
		}

		// add new
		if (tile.shape == TileShape.FULL) {
			Collider2DWrapper tmp = components.Get (x - 1, y) as Collider2DWrapper;
			if (tmp != null && tmp.height == 1 && tmp.collider != null) {
				tmp.width += 1;

				BoxCollider2D box = tmp.collider as BoxCollider2D;
				box.transform.localPosition = new Vector2 (box.transform.localPosition.x + 0.5f, box.transform.localPosition.y);
				box.size = new Vector2 (box.size.x + 1, 1);

				components.Set (x, y, tmp);
				return;
			}

			tmp = components.Get (x + 1, y) as Collider2DWrapper;
			if (tmp != null && tmp.height == 1 && tmp.collider != null) {
				tmp.width += 1;
				tmp.x -= 1;

				BoxCollider2D box = tmp.collider as BoxCollider2D;
				box.transform.localPosition = new Vector2 (box.transform.localPosition.x - 0.5f, box.transform.localPosition.y);
				box.size = new Vector2 (box.size.x + 1, 1);

				components.Set (x, y, tmp);
				return;
			}
			
			tmp = components.Get (x, y - 1) as Collider2DWrapper;
			if (tmp != null && tmp.width == 1 && tmp.collider != null) {
				tmp.height += 1;

				BoxCollider2D box = tmp.collider as BoxCollider2D;
				box.transform.localPosition = new Vector2 (box.transform.localPosition.x, box.transform.localPosition.y + 0.5f);
				box.size = new Vector2 (1, box.size.y + 1);

				components.Set (x, y, tmp);
				return;
			}

			tmp = components.Get (x, y + 1) as Collider2DWrapper;
			if (tmp != null && tmp.width == 1 && tmp.collider != null) {
				tmp.height += 1;
				tmp.y -= 1;

				BoxCollider2D box = tmp.collider as BoxCollider2D;
				box.transform.localPosition = new Vector2 (box.transform.localPosition.x, box.transform.localPosition.y - 0.5f);
				box.size = new Vector2 (1, box.size.y + 1);

				components.Set (x, y, tmp);
				return;
			}

			tmp = new Collider2DWrapper ();
			tmp.x = x;
			tmp.y = y;
			tmp.width = 1;
			tmp.height = 1;

			GameObject newObj = new GameObject ("box collider");

			BoxCollider2D newNewBox = newObj.AddComponent<BoxCollider2D> ();
			newNewBox.size = new Vector2 (1, 1);

			newObj.transform.SetParent (transform);
			newObj.transform.localPosition = grid.TileCenterInTransform (x, y);
			newObj.transform.localScale = new Vector3 (grid.tileSize, grid.tileSize, 1);

			components.Set (x, y, tmp);
		}*/
	}
	public override void OnRegionChange(int regionX, int regionY) {

	}

	public override void OnTileSizeChange() {

	}
}