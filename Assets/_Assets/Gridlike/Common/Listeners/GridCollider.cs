using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {
	
    /* OPTIMIZATION
     * 
     * Apply size change to the collider at the end of any given operation
     * Apply position change to the collider at the end of any given operation
     * No destroy or instantiation, pooling
     * Limit access to the infinite grid and access directly to regions
     */

	[AddComponentMenu("Gridlike/Grid collider")]
	public class GridCollider : GridListener {

		[HideInInspector] [SerializeField] InfiniteComponentGrid components;

		[HideInInspector] [SerializeField] GameObject containerGO;

        /*HashSet<GridColliderPart> parts;
        void ResetModified() {
            if(parts == null) parts = new HashSet<GridColliderPart>();
            else parts.Clear();
        }*/

		public override void OnDestroy() {
			base.OnDestroy ();

			if(Application.isPlaying) 
				Destroy (containerGO);
			else
				DestroyImmediate (containerGO);
			containerGO = null;
		}

		public override void ResetListener() {
			if (components == null) {
				components = new InfiniteComponentGrid (Grid.REGION_SIZE);
			}

			if (containerGO == null) {
				containerGO = new GameObject ("colliders");
				containerGO.transform.SetParent (transform, false);
			}

			base.ResetListener ();
		}

		public override void OnSet(int x, int y, Tile tile) {
			// clear previous
			ClearTile(x, y);

			TileInfo info = grid.atlas [tile.id];

			// add new
			if (info.shape != TileShape.EMPTY) {

				GridColliderPart wrapper = null;

				// HORIZONTAL GROWTH
				if (info.shape != TileShape.LEFT_ONEWAY && info.shape != TileShape.RIGHT_ONEWAY) {
					bool expanded = false;

					GridColliderPart left = components.Get (x - 1, y) as GridColliderPart;
					if (left != null && left.Compatible (info) && !left.isVertical && !info.isVertical) {
						left.SetSize (left.width + 1, 1);
						wrapper = left;

						components.Set (x, y, left);

						expanded = true;
					}
					
					GridColliderPart right = components.Get (x + 1, y) as GridColliderPart;
					if (right != null && right.Compatible (info) && !right.isVertical && !info.isVertical) {
						if (!expanded) {
							right.bottomLeftX -= 1;
                            right.SetSize (right.width + 1, 1);
							wrapper = right;

							components.Set (x, y, right);

							wrapper.ResetSizeAndPosition (grid);
							return;
						} else {
							left.SetSize (left.width + right.width, 1);

							for (int i = right.bottomLeftX; i < right.bottomLeftX + right.width; i++) {
								components.Set (i, y, left);
							}

							if (Application.isPlaying)
								Destroy (right.gameObject);
							else
								DestroyImmediate (right.gameObject);

							wrapper.ResetSizeAndPosition (grid);
							return;
						}
					}

					if (expanded) {
						wrapper.ResetSizeAndPosition (grid);
						return;
					}
				}

				// VERTICAL GROWTH
				if (info.shape != TileShape.FULL && info.shape != TileShape.UP_ONEWAY && info.shape != TileShape.DOWN_ONEWAY) {
					bool expanded = false;

					GridColliderPart down = components.Get (x, y - 1) as GridColliderPart;
					if (down != null && down.Compatible (info) && down.isVertical && info.isVertical) {
						down.SetSize (1, down.height + 1);
						wrapper = down;

						components.Set (x, y, down);

						expanded = true;
					}

					GridColliderPart up = components.Get (x, y + 1) as GridColliderPart;

					if (up != null && up.Compatible (info) && up.isVertical && info.isVertical) {
						if (!expanded) {
							up.bottomLeftY -= 1;
							up.SetSize (1, up.height + 1);
							wrapper = up;

							components.Set (x, y, up);

							wrapper.ResetSizeAndPosition (grid);
							return;
						} else {
							down.SetSize (1, down.height + up.height);

							for (int i = up.bottomLeftY; i < up.bottomLeftY + up.width; i++) {
								components.Set (i, y, down);
							}

							if (Application.isPlaying)
								Destroy (up.gameObject);
							else
								DestroyImmediate (up.gameObject);

							wrapper.ResetSizeAndPosition (grid);
							return;
						}
					}

					if (expanded) {
						wrapper.ResetSizeAndPosition (grid);
						return;
					}
				}

				// NO EXPANSE, CREATE NEW
				wrapper = GridColliderPart.CreateColliderPart (containerGO, grid, info, x, y, 1, 1);
				components.Set (x, y, wrapper);

				wrapper.ResetSizeAndPosition (grid);
			}
		}

		public override void OnHideRegion(int X, int Y) {
			int bx = X * Grid.REGION_SIZE;
			int by = Y * Grid.REGION_SIZE;

			FiniteComponentGrid regionComponents = components.GetRegion(X, Y);

			if (regionComponents != null) {
				for (int i = 0; i < Grid.REGION_SIZE; i++) {
					for (int j = 0; j < Grid.REGION_SIZE; j++) {
						GridColliderPart wrapper = regionComponents.Get (i, j) as GridColliderPart;

						if (wrapper != null) {
							if (wrapper.bottomLeftX >= bx && wrapper.bottomLeftX + wrapper.width <= bx + Grid.REGION_SIZE
							   && wrapper.bottomLeftY >= by && wrapper.bottomLeftY + wrapper.height <= by + Grid.REGION_SIZE) {

								if (Application.isPlaying)
									Destroy (wrapper.gameObject);
								else
									DestroyImmediate (wrapper.gameObject);
							} else if (wrapper.isVertical) {
								if (wrapper.bottomLeftY < by) {
									int topY = wrapper.bottomLeftY + wrapper.height;

									wrapper.SetSize (1, by - wrapper.bottomLeftY);
									wrapper.ResetSizeAndPosition (grid);
								} else if (wrapper.bottomLeftY + wrapper.height > by + Grid.REGION_SIZE) {
									wrapper.SetSize (1, wrapper.bottomLeftY + wrapper.height - (by + Grid.REGION_SIZE));
									wrapper.bottomLeftY = by + Grid.REGION_SIZE;
									wrapper.ResetSizeAndPosition (grid);
								}
							} else if (!wrapper.isVertical) {
								if (wrapper.bottomLeftX < bx) {
									int rightX = wrapper.bottomLeftX + wrapper.width;

									wrapper.SetSize (bx - wrapper.bottomLeftX, 1);
									wrapper.ResetSizeAndPosition (grid);
								} else if (wrapper.bottomLeftX + wrapper.width > bx + Grid.REGION_SIZE) {
									wrapper.SetSize (wrapper.bottomLeftX + wrapper.width - (bx + Grid.REGION_SIZE), 1);
									wrapper.bottomLeftX = bx + Grid.REGION_SIZE;
									wrapper.ResetSizeAndPosition (grid);
								}
							}
						}
					}
				}

				components.ClearRegion (X, Y);
			}
		}
		public override void OnShowRegion(int regionX, int regionY) {
			int bx = regionX * Grid.REGION_SIZE;
			int by = regionY * Grid.REGION_SIZE;

			FiniteGrid region = grid.GetRegion (regionX, regionY);
			FiniteComponentGrid regionComponents = components.GetOrCreateRegion (regionX, regionY);

			for (int y = 0; y < Grid.REGION_SIZE; y++) {
				GridColliderPart currentWrapper = components.Get (bx - 1, y + by) as GridColliderPart;

				for (int x = 0; x < Grid.REGION_SIZE - 1; x++) {
					Tile tile = region.Get (x, y);

					if (tile != null) {
						TileInfo info = grid.atlas [tile.id];

						if (currentWrapper != null && currentWrapper.Compatible (info) && !info.isVertical && !currentWrapper.isVertical) {
							currentWrapper.width++;
						} else {
							if (currentWrapper != null)
								currentWrapper.ResetSizeAndPosition (grid);

							if (info.shape != TileShape.EMPTY && !info.isVertical) {
								currentWrapper = GridColliderPart.CreateColliderPart (containerGO, grid, info, bx + x, by + y, 1, 1);
							} else {
								currentWrapper = null;
							}
						}

						regionComponents.Set (x, y, currentWrapper);
					} else {
						if (currentWrapper != null) {
							currentWrapper.ResetSizeAndPosition (grid);
							currentWrapper = null;
						}

						regionComponents.Set (x, y, null);
					}
				}

				if (currentWrapper != null) currentWrapper.ResetSizeAndPosition (grid);

				Tile edgeTile = region.Get (Grid.REGION_SIZE - 1, y);
				if (edgeTile != null) OnSet (bx + Grid.REGION_SIZE - 1, by + y, edgeTile);
			}

			for (int x = 0; x < Grid.REGION_SIZE; x++) {
				GridColliderPart currentWrapper = components.Get (bx + x, by - 1) as GridColliderPart;

				for (int y = 0; y < Grid.REGION_SIZE - 1; y++) {
					Tile tile = region.Get (x, y);

					if (tile != null) {
						TileInfo info = grid.atlas [tile.id];

						if (currentWrapper != null && currentWrapper.Compatible (info) && info.isVertical && currentWrapper.isVertical) {
							currentWrapper.height++;

							regionComponents.Set (x, y, currentWrapper);
						} else {
							if (currentWrapper != null)
								currentWrapper.ResetSizeAndPosition (grid);

							if (info.shape != TileShape.EMPTY && info.isVertical) {
								currentWrapper = GridColliderPart.CreateColliderPart (containerGO, grid, info, bx + x, by + y, 1, 1);

								regionComponents.Set (x, y, currentWrapper);
							} else {
								currentWrapper = null;
							}
						}
					} else {
						if (currentWrapper != null) {
							currentWrapper.ResetSizeAndPosition (grid);
							currentWrapper = null;
						}
					}
				}

				if (currentWrapper != null) currentWrapper.ResetSizeAndPosition (grid);

				Tile edgeTile = region.Get (x, Grid.REGION_SIZE - 1);
				if (edgeTile != null && grid.atlas[edgeTile.id].isVertical) OnSet (bx + x, by + Grid.REGION_SIZE - 1, edgeTile);
			}
        }

		void ClearTile(int x, int y) {
			GridColliderPart wrapper = components.Get (x, y) as GridColliderPart;

            _ClearTile(wrapper, x, y);

			components.Set (x, y, null);
		}

        void _ClearTile(GridColliderPart wrapper, int x, int y) {
            if (wrapper != null) {
                if (wrapper.width == 1) {
                    if (wrapper.height == 1) {
                        if (Application.isPlaying)
                            Destroy (wrapper.gameObject);
                        else
                            DestroyImmediate (wrapper.gameObject);
                    } else {
                        if (wrapper.bottomLeftY == y) {
                            wrapper.bottomLeftY += 1;

                            wrapper.SetSize(wrapper.width, wrapper.height - 1);
							wrapper.ResetSizeAndPosition(grid);
                        } else {
                            int endY = wrapper.bottomLeftY + wrapper.height - 1;

                            wrapper.SetSize(wrapper.width, y - wrapper.bottomLeftY);
							wrapper.ResetSizeAndPosition(grid);

                            if (endY != y) {
                                GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, grid.atlas[wrapper.id], x, y + 1, 1, endY - y);
								part.ResetSizeAndPosition (grid);

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
						wrapper.ResetSizeAndPosition(grid);
                    } else {
                        int endX = wrapper.bottomLeftX + wrapper.width - 1;

                        wrapper.SetSize (x - wrapper.bottomLeftX, wrapper.height);
						wrapper.ResetSizeAndPosition(grid);

                        if (endX != x) {
							GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, grid.atlas[wrapper.id], x + 1, y, endX - x, 1);
							part.ResetSizeAndPosition (grid);

                            for (int i = x + 1; i <= endX; i++) {
                                components.Set (i, y, part);
                            }
                        }
                    }
                }
            }
        }
	}
}