using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpriteRenderer : GridListener {

	[HideInInspector]
	[SerializeField]
	InfiniteComponentGrid components;

	public override void ResetGrid() {
		base.ResetGrid ();

		if (components == null) {
			components = new InfiniteComponentGrid (Grid.REGION_SIZE);
		}
	}

	public override void OnSet(int x, int y, Tile tile) {
		Component renderer = components.Get (x, y);

		switch (tile.shape) {
		case TileShape.EMPTY: {
				if (renderer != null) ClearSprite (renderer, x, y);

				break;
			}
		case TileShape.UP_ONEWAY:
		case TileShape.RIGHT_ONEWAY:
		case TileShape.DOWN_ONEWAY:
		case TileShape.LEFT_ONEWAY:
		case TileShape.FULL: {
				if (renderer == null) {
					renderer = CreateSprite (x, y);

					components.Set (x, y, renderer);
				} else if (renderer is GridSpriteTriangle) {
					if ((renderer as GridSpriteTriangle).width != 1 || (renderer as GridSpriteTriangle).height != 1) {
						SplitTriangle (renderer as GridSpriteTriangle, x, y);

						renderer = CreateSprite (x, y);

						components.Set (x, y, renderer);
					} else {
						DestroyImmediate (renderer);

						renderer = (renderer as GridSpriteTriangle).spriteRenderer;

						components.Set (x, y, renderer);
					}
				}

				(renderer as SpriteRenderer).sprite = grid.atlas.GetSprite(tile.id, tile.subId, tile.shape);

				break;
			}
		case TileShape.DOWN_LEFT_TRIANGLE:
		case TileShape.DOWN_RIGHT_TRIANGLE:
		case TileShape.UP_LEFT_TRIANGLE:
		case TileShape.UP_RIGHT_TRIANGLE: {
				if (renderer != null) ClearSprite (renderer, x, y);

				bool expand = false;
				int size = 1;
				int heigth = 1;

				// up
				Tile other = grid.Get (x, y + 1);

				if (other != null && tile.id == other.id && tile.subId == other.subId) {
					GridSpriteTriangle otherTriangle = components.Get (x, y + 1) as GridSpriteTriangle;

					if (otherTriangle != null && otherTriangle.width == 1) {
						components.Set (x, y, otherTriangle);

						expand = true;

						renderer = otherTriangle.spriteRenderer;

						otherTriangle.transform.localPosition -= new Vector3 (0, 0.5f, 0);
						otherTriangle.bottomLeftY -= 1;
						otherTriangle.height += 1;

						size = otherTriangle.height;
					}
				}

				// right
				if (!expand) {
					other = grid.Get (x + 1, y);

					if (other != null && tile.id == other.id && tile.subId == other.subId) {
						GridSpriteTriangle otherTriangle = components.Get (x + 1, y) as GridSpriteTriangle;

						if (otherTriangle != null && otherTriangle.height == 1) {
							components.Set (x, y, otherTriangle);

							expand = true;

							renderer = otherTriangle.spriteRenderer;

							otherTriangle.transform.localPosition -= new Vector3 (0.5f, 0, 0);
							otherTriangle.bottomLeftX -= 1;
							otherTriangle.width += 1;

							size = otherTriangle.width;
						}
					}
				}

				// down
				if (!expand) {
					other = grid.Get (x, y - 1);

					if (other != null && tile.id == other.id && tile.subId == other.subId) {
						GridSpriteTriangle otherTriangle = components.Get (x, y - 1) as GridSpriteTriangle;

						if (otherTriangle != null && otherTriangle.width == 1) {
							components.Set (x, y, otherTriangle);

							expand = true;

							renderer = otherTriangle.spriteRenderer;

							otherTriangle.transform.localPosition += new Vector3 (0, 0.5f, 0);
							otherTriangle.height += 1;

							size = otherTriangle.height;
						}
					}
				}

					// left
				if (!expand) {
					other = grid.Get (x - 1, y);

					if (other != null && tile.id == other.id && tile.subId == other.subId) {
						GridSpriteTriangle otherTriangle = components.Get (x - 1, y) as GridSpriteTriangle;

						if (otherTriangle != null && otherTriangle.height == 1) {
							components.Set (x, y, otherTriangle);

							expand = true;

							renderer = otherTriangle.spriteRenderer;

							otherTriangle.transform.localPosition += new Vector3 (0.5f, 0, 0);
							otherTriangle.width += 1;

							size = otherTriangle.width;
						}
					}
				}

				if (!expand) {
					GridSpriteTriangle triangle = GridSpriteTriangle.CreateSpriteTriangle (gameObject, grid, x, y);

					components.Set (x, y, triangle);

					renderer = triangle.spriteRenderer;
				}

				(renderer as SpriteRenderer).sprite = grid.atlas.GetSprite(tile.id, tile.subId, tile.shape, size);

				break;
			}
		}
	}

	public override void OnTileSizeChange () {

	}

	void ClearSprite(Component renderer, int x, int y) {
		if (renderer is GridSpriteTriangle && ((renderer as GridSpriteTriangle).width != 1 || (renderer as GridSpriteTriangle).height != 1)) {
			SplitTriangle (renderer as GridSpriteTriangle, x, y);
		} else {
			DestroyImmediate (renderer.gameObject);
		}

		components.Set (x, y, null);
	}

	SpriteRenderer CreateSprite(int x, int y) {
		GameObject obj = new GameObject ("sprite x=" + x + " y=" + y);

		SpriteRenderer renderer = obj.AddComponent<SpriteRenderer> ();

		obj.transform.SetParent (transform);
		obj.transform.localPosition = grid.TileCenterInTransform (x, y);
		obj.transform.localScale = new Vector3 (grid.tileSize, grid.tileSize, 1);

		return renderer;
	}
	void SplitTriangle(GridSpriteTriangle triangle, int x, int y) {
		if (triangle.width != 1) {
			if (triangle.bottomLeftX == x) {
				Tile other = grid.Get(x + 1, y);

				triangle.width -= 1;
				triangle.bottomLeftX += 1;
				triangle.transform.localPosition += new Vector3 (0.5f, 0, 0);
				triangle.spriteRenderer.sprite = grid.atlas.GetSprite(other.id, other.subId, other.shape, triangle.width);
			} else {
				int endX = triangle.bottomLeftX + triangle.width - 1;
				Tile other = grid.Get(triangle.bottomLeftX, y);

				triangle.width = x - triangle.bottomLeftX;
				triangle.transform.localPosition -= new Vector3 ((endX - x + 1)/2f, 0, 0);
				triangle.spriteRenderer.sprite = grid.atlas.GetSprite(other.id, other.subId, other.shape, triangle.width);

				if (endX != x) {
					GridSpriteTriangle otherTriangle = GridSpriteTriangle.CreateSpriteTriangle(gameObject, grid, x + 1, y);
					otherTriangle.transform.localPosition += new Vector3 ((endX - x - 1) / 2f, 0, 0);
					otherTriangle.width = endX - x;
					otherTriangle.spriteRenderer.sprite = grid.atlas.GetSprite (other.id, other.subId, other.shape, otherTriangle.width);

					for (int i = x + 1; i <= endX; i++) {
						components.Set (i, y, otherTriangle);
					}
				}
			} 
		} else {
			if (triangle.bottomLeftY == y) {
				Tile other = grid.Get(x, y + 1);

				triangle.height -= 1;
				triangle.bottomLeftY += 1;
				triangle.transform.localPosition += new Vector3 (0, 0.5f, 0);
				triangle.spriteRenderer.sprite = grid.atlas.GetSprite(other.id, other.subId, other.shape, triangle.height);
			} else {
				int endY = triangle.bottomLeftY + triangle.height - 1;
				Tile other = grid.Get(x, triangle.bottomLeftY);

				triangle.height = y - triangle.bottomLeftY;
				triangle.transform.localPosition -= new Vector3 (0, (endY - y + 1)/2f, 0);
				triangle.spriteRenderer.sprite = grid.atlas.GetSprite(other.id, other.subId, other.shape, y - triangle.bottomLeftY);

				if (endY == y) {
					GridSpriteTriangle otherTriangle = GridSpriteTriangle.CreateSpriteTriangle(gameObject, grid, x, y + 1);
					otherTriangle.transform.localPosition += new Vector3 (0, (endY - y - 1) / 2f, 0);
					otherTriangle.width = endY - y;
					otherTriangle.spriteRenderer.sprite = grid.atlas.GetSprite (other.id, other.subId, other.shape, otherTriangle.height);

					for (int i = y + 1; i <= endY; i++) {
						components.Set (x, i, otherTriangle);
					}
				}
			}
		}
	}
}