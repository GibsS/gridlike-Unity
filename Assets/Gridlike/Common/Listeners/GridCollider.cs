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

        HashSet<GridColliderPart> parts;
        void ResetModified() {
            if(parts == null) parts = new HashSet<GridColliderPart>();
            else parts.Clear();
        }

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
				// HORIZONTAL GROWTH
				if (info.shape != TileShape.LEFT_ONEWAY && info.shape != TileShape.RIGHT_ONEWAY) {
					bool expanded = false;

					GridColliderPart left = components.Get (x - 1, y) as GridColliderPart;
					if (left != null && left.Compatible (info) && !left.isVertical && !info.isVertical) {
                        left.SetSize (left.width + 1, 1);
                        left.ResetPosition(grid);

						components.Set (x, y, left);

						expanded = true;
					}
					
					GridColliderPart right = components.Get (x + 1, y) as GridColliderPart;
					if (right != null && right.Compatible (info) && !right.isVertical && !info.isVertical) {
						if (!expanded) {
							right.bottomLeftX -= 1;

                            right.SetSize (right.width + 1, 1);
                            right.ResetPosition(grid);

							components.Set (x, y, right);

							return;
						} else {
							left.SetSize (left.width + right.width, 1);
                            left.ResetPosition(grid);

							for (int i = right.bottomLeftX; i < right.bottomLeftX + right.width; i++) {
								components.Set (i, y, left);
							}

							if (Application.isPlaying)
								Destroy (right.gameObject);
							else
								DestroyImmediate (right.gameObject);

							return;
						}
					}

					if (expanded) return;
				}

				// VERTICAL GROWTH
				if (info.shape != TileShape.FULL && info.shape != TileShape.UP_ONEWAY && info.shape != TileShape.DOWN_ONEWAY) {
					bool expanded = false;

					GridColliderPart down = components.Get (x, y - 1) as GridColliderPart;
					if (down != null && down.Compatible (info) && down.isVertical && info.isVertical) {
						down.SetSize (1, down.height + 1);
                        down.ResetPosition(grid);

						components.Set (x, y, down);

						expanded = true;
					}

					GridColliderPart up = components.Get (x, y + 1) as GridColliderPart;

					if (up != null && up.Compatible (info) && up.isVertical && info.isVertical) {
						if (!expanded) {
							up.bottomLeftY -= 1;

							up.SetSize (1, up.height + 1);
                            up.ResetPosition(grid);

							components.Set (x, y, up);

							return;
						} else {
							down.SetSize (1, down.height + up.height);
                            down.ResetPosition(grid);

							for (int i = up.bottomLeftY; i < up.bottomLeftY + up.width; i++) {
								components.Set (i, y, down);
							}

							if (Application.isPlaying)
								Destroy (up.gameObject);
							else
								DestroyImmediate (up.gameObject);

							return;
						}
					}

					if (expanded) return;
				}

				// NO EXPANSE, CREATE NEW
				components.Set (x, y, GridColliderPart.CreateColliderPart (containerGO, grid, info, x, y, 1, 1));
			}
		}

		public override void OnHideRegion(int X, int Y) {
			int startX = X * Grid.REGION_SIZE;
			int endX = (X + 1) * Grid.REGION_SIZE;
			int startY = Y * Grid.REGION_SIZE;
			int endY = (Y + 1) * Grid.REGION_SIZE;

            FiniteComponentGrid region = components.GetRegion(X, Y);

			for (int i = startX; i < endX; i++) {
				for (int j = startY; j < endY; j++) {
                    GridColliderPart wrapper = region.Get(i - startX, j - startY) as GridColliderPart;
                    
					_ClearTile (wrapper, i, j);

                    region.Set(i - startX, j - startY, null);
				}
			}
		}
        /*public virtual void OnShowRegion(int regionX, int regionY) {
            if (Application.isPlaying) {
                StartCoroutine (_OnShowRegion (regionX, regionY));
            } else {  
                
            }
        }*/

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
                            wrapper.ResetPosition(grid);
                        } else {
                            int endY = wrapper.bottomLeftY + wrapper.height - 1;

                            wrapper.SetSize(wrapper.width, y - wrapper.bottomLeftY);
                            wrapper.ResetPosition(grid);

                            if (endY != y) {
                                GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, grid.atlas[wrapper.id], x, y + 1, 1, endY - y);

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
                        wrapper.ResetPosition(grid);
                    } else {
                        int endX = wrapper.bottomLeftX + wrapper.width - 1;

                        wrapper.SetSize (x - wrapper.bottomLeftX, wrapper.height);
                        wrapper.ResetPosition(grid);

                        if (endX != x) {
                            GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, grid.atlas[wrapper.id], x + 1, y, endX - x, 1);

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