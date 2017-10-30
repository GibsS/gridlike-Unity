using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {
	
	[AddComponentMenu("Gridlike/Grid collider")]
	public class GridCollider : GridListener {

		[HideInInspector]
		[SerializeField]
		InfiniteComponentGrid components;

		[HideInInspector]
		[SerializeField] 
		GameObject containerGO;

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
						left.transform.localPosition = new Vector2 (left.transform.localPosition.x + 0.5f, left.transform.localPosition.y);
						left.SetSize (left.width + 1, 1);

						components.Set (x, y, left);

						expanded = true;
					}
					
					GridColliderPart right = components.Get (x + 1, y) as GridColliderPart;
					if (right != null && right.Compatible (info) && !right.isVertical && !info.isVertical) {
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
						down.transform.localPosition = new Vector2 (down.transform.localPosition.x, down.transform.localPosition.y + 0.5f);
						down.SetSize (1, down.height + 1);

						components.Set (x, y, down);

						expanded = true;
					}

					GridColliderPart up = components.Get (x, y + 1) as GridColliderPart;
					if (up != null && up.Compatible (info) && up.isVertical && info.isVertical) {
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

			for (int i = startX; i < endX; i++) {
				for (int j = startY; j < endY; j++) {
					ClearTile (i, j);
				}
			}
		}

		void ClearTile(int x, int y) {
			GridColliderPart wrapper = components.Get (x, y) as GridColliderPart;

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
							wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x, wrapper.transform.localPosition.y + 0.5f);
						} else {
							int endY = wrapper.bottomLeftY + wrapper.height - 1;

							wrapper.SetSize(wrapper.width, y - wrapper.bottomLeftY);
							wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x, wrapper.transform.localPosition.y - (endY - y + 1f)/2f);

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
						wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x + 0.5f, wrapper.transform.localPosition.y);
					} else {
						int endX = wrapper.bottomLeftX + wrapper.width - 1;

						wrapper.SetSize (x - wrapper.bottomLeftX, wrapper.height);
						wrapper.transform.localPosition = new Vector2 (wrapper.transform.localPosition.x - (endX - x + 1f)/2f, wrapper.transform.localPosition.y);

						if (endX != x) {
							GridColliderPart part = GridColliderPart.CreateColliderPart (containerGO, grid, grid.atlas[wrapper.id], x + 1, y, endX - x, 1);

							for (int i = x + 1; i <= endX; i++) {
								components.Set (i, y, part);
							}
						}
					}
				}
			}

			components.Set (x, y, null);
		}
	}
}