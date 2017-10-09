using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpriteRenderer : GridListener {

	[HideInInspector]
	[SerializeField]
	InfiniteComponentGrid components;

	[HideInInspector]
	[SerializeField] 
	GameObject containerGO;

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
			containerGO = new GameObject ("sprites");
			containerGO.transform.SetParent (transform);
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

				// up
				Tile other = grid.Get (x, y + 1);

				GridSpriteTriangle upTriangle = null;
				if (other != null && tile.id == other.id && tile.subId == other.subId) {
					upTriangle = components.Get (x, y + 1) as GridSpriteTriangle;

					if (upTriangle != null && upTriangle.width == 1) {
						components.Set (x, y, upTriangle);

						expand = true;

						renderer = upTriangle.spriteRenderer;

						upTriangle.transform.localPosition -= new Vector3 (0, 0.5f, 0);
						upTriangle.bottomLeftY -= 1;
						upTriangle.height += 1;

						size = upTriangle.height;
					}
				}

				// down
				other = grid.Get (x, y - 1);

				if (other != null && tile.id == other.id && tile.subId == other.subId) {
					GridSpriteTriangle otherTriangle = components.Get (x, y - 1) as GridSpriteTriangle;

					if (otherTriangle != null && otherTriangle.width == 1) {
						if (expand) {
							for (int i = otherTriangle.bottomLeftY; i <= y; i++) {
								components.Set (x, i, upTriangle);
							}

							upTriangle.bottomLeftY = otherTriangle.bottomLeftY;
							upTriangle.height += otherTriangle.height;
							upTriangle.transform.localPosition -= new Vector3(0, otherTriangle.height/2f, 0);

							DestroyImmediate (otherTriangle.gameObject);

							size = upTriangle.height;
						} else {
							components.Set (x, y, otherTriangle);

							expand = true;

							renderer = otherTriangle.spriteRenderer;

							otherTriangle.transform.localPosition += new Vector3 (0, 0.5f, 0);
							otherTriangle.height += 1;

							size = otherTriangle.height;
						}
					}
				}

				// right
				if (!expand) {
					other = grid.Get (x + 1, y);

					GridSpriteTriangle rightTriangle = null;
					if (other != null && tile.id == other.id && tile.subId == other.subId) {
						rightTriangle = components.Get (x + 1, y) as GridSpriteTriangle;

						if (rightTriangle != null && rightTriangle.height == 1) {
							components.Set (x, y, rightTriangle);

							expand = true;

							renderer = rightTriangle.spriteRenderer;

							rightTriangle.transform.localPosition -= new Vector3 (0.5f, 0, 0);
							rightTriangle.bottomLeftX -= 1;
							rightTriangle.width += 1;

							size = rightTriangle.width;
						}
					}

					// left
					other = grid.Get (x - 1, y);

					if (other != null && tile.id == other.id && tile.subId == other.subId) {
						GridSpriteTriangle otherTriangle = components.Get (x - 1, y) as GridSpriteTriangle;

						if (otherTriangle != null && otherTriangle.height == 1) {
							if (expand) {
								for (int i = otherTriangle.bottomLeftX; i <= x; i++) {
									components.Set (i, y, rightTriangle);
								}

								rightTriangle.bottomLeftX = otherTriangle.bottomLeftX;
								rightTriangle.width += otherTriangle.width;
								rightTriangle.transform.localPosition -= new Vector3(otherTriangle.width/2f, 0, 0);

								DestroyImmediate (otherTriangle.gameObject);

								size = rightTriangle.width;
							} else {
								components.Set (x, y, otherTriangle);

								expand = true;

								renderer = otherTriangle.spriteRenderer;

								otherTriangle.transform.localPosition += new Vector3 (0.5f, 0, 0);
								otherTriangle.width += 1;

								size = otherTriangle.width;
							}
						}
					}
				}

				if (!expand) {
					GridSpriteTriangle triangle = GridSpriteTriangle.CreateSpriteTriangle (containerGO, grid, x, y);

					components.Set (x, y, triangle);

					renderer = triangle.spriteRenderer;
				}

				(renderer as SpriteRenderer).sprite = grid.atlas.GetSprite(tile.id, tile.subId, tile.shape, size);

				break;
			}
		}
	}

	public override void OnHideRegion(int X, int Y) {
		int startX = X * Grid.REGION_SIZE;
		int endX = (X + 1) * Grid.REGION_SIZE;
		int startY = Y * Grid.REGION_SIZE;
		int endY = (Y + 1) * Grid.REGION_SIZE;

		for (int i = startX; i < endX; i++) {
			for (int j = startY; j < endY; j++) {
				ClearSprite (components.Get(i, j), i, j);
			}
		}
	}

	public override void OnTileSizeChange () {
		Debug.Log ("[GridSpriteRenderer.OnTileSizeChange] NOT IMPLEMENTED");
	}

	void ClearSprite(Component renderer, int x, int y) {
		if (renderer != null) {
			if (renderer is GridSpriteTriangle && ((renderer as GridSpriteTriangle).width != 1 || (renderer as GridSpriteTriangle).height != 1)) {
				SplitTriangle (renderer as GridSpriteTriangle, x, y);
			} else {
				DestroyImmediate (renderer.gameObject);
			}

			components.Set (x, y, null);
		}
	}

	SpriteRenderer CreateSprite(int x, int y) {
		GameObject obj = new GameObject ("sprite x=" + x + " y=" + y);
		obj.transform.SetParent (containerGO.transform);

		SpriteRenderer renderer = obj.AddComponent<SpriteRenderer> ();

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
					GridSpriteTriangle otherTriangle = GridSpriteTriangle.CreateSpriteTriangle(containerGO, grid, x + 1, y);
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

				if (endY != y) {
					GridSpriteTriangle otherTriangle = GridSpriteTriangle.CreateSpriteTriangle(containerGO, grid, x, y + 1);
					otherTriangle.transform.localPosition += new Vector3 (0, (endY - y - 1) / 2f, 0);
					otherTriangle.height = endY - y;
					otherTriangle.spriteRenderer.sprite = grid.atlas.GetSprite (other.id, other.subId, other.shape, otherTriangle.height);

					for (int i = y + 1; i <= endY; i++) {
						components.Set (x, i, otherTriangle);
					}
				}
			}
		}
	}
}