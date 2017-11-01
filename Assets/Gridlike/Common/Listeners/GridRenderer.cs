using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {
	
	[Serializable]
	public class PositionRegionRenderer {

		public int regionX;
		public int regionY;
		public RegionMeshRenderer mesh;
	}

	[AddComponentMenu("Gridlike/Grid renderer")]
	public class GridRenderer : GridListener {

		[HideInInspector] [SerializeField] List<PositionRegionRenderer> regionRenderers;
		[HideInInspector] [SerializeField] InfiniteComponentGrid triangles;

		[HideInInspector] [SerializeField] GameObject containerGO;

		ComponentPool<RegionMeshRenderer> meshes;

		public override void OnDestroy() {
			base.OnDestroy ();

			if (Application.isPlaying)
				Destroy (containerGO);
			else
				DestroyImmediate (containerGO);
			containerGO = null;

			if(meshes != null) meshes.Clear ();
		}
		public override void Awake() {
			base.Awake ();

			if (Application.isPlaying) {
				GridlikePools.Initialize ();
			}
		}

		public override void ResetListener() {
			if (regionRenderers == null) {
				regionRenderers = new List<PositionRegionRenderer> ();
			}

			if (triangles == null) {
				triangles = new InfiniteComponentGrid (Grid.REGION_SIZE);
			}

			if (containerGO == null) {
				containerGO = new GameObject ("sprites");
				containerGO.transform.SetParent (transform, false);
			}

			base.ResetListener ();
		}

		RegionMeshRenderer CreateRegionRenderer() {
			RegionMeshRenderer rend;
			if (Application.isPlaying) {
				rend = GridlikePools.renderers.Get ();
			} else {
				rend = RegionMeshRenderer.Create (Grid.REGION_SIZE);
			}

			rend.Initialize (grid.atlas.spriteSheet, grid.atlas.tilePixelSize);

			return rend;
		}

		PositionRegionRenderer GetContainingRegionRenderer(int x, int y) {
			return GetRegionRenderer (Mathf.FloorToInt(((float)x) / Grid.REGION_SIZE), Mathf.FloorToInt(((float)y) / Grid.REGION_SIZE));
		}
		PositionRegionRenderer GetRegionRenderer(int regionX, int regionY) {
			var rend = regionRenderers.Find (e => e.regionX == regionX && e.regionY == regionY);

			if (rend == null) {
				RegionMeshRenderer regionRenderer = CreateRegionRenderer ();

				regionRenderer.transform.SetParent (containerGO.transform);
				regionRenderer.transform.localPosition = new Vector2 (regionX * Grid.REGION_SIZE, regionY * Grid.REGION_SIZE);

				rend = new PositionRegionRenderer {
					regionX = regionX,
					regionY = regionY,
					mesh = regionRenderer
				};

				regionRenderers.Add (rend);

				return rend;
			} else {
				return rend;
			}
		}
		void ClearRegionRenderer(int regionX, int regionY) {
			var rend = regionRenderers.Find (e => e.regionX == regionX && e.regionY == regionY);

			if (rend != null) {
				if (Application.isPlaying) {
					for (int i = 0; i < Grid.REGION_SIZE; i++) {
						for (int j = 0; j < Grid.REGION_SIZE; j++) {
							rend.mesh.SetTile (i, j, grid.atlas.emptySprite);
						}
					}
					meshes.Free (rend.mesh);
				} else {
					rend.mesh.Destroy();
				}

				regionRenderers.Remove (rend);
			}
		}

		public override void OnSet(int x, int y, Tile tile) {
			TileInfo info = grid.atlas [tile.id];

			switch (info.shape) {
			case TileShape.EMPTY: {
					PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);

					renderer.mesh.PrepareUV ();
					Clear (renderer, x, y);
					renderer.mesh.ApplyUV ();
					break;
				}
			case TileShape.UP_ONEWAY:
			case TileShape.RIGHT_ONEWAY:
			case TileShape.DOWN_ONEWAY:
			case TileShape.LEFT_ONEWAY:
			case TileShape.FULL: {
					PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);

					renderer.mesh.PrepareUV ();
					Clear (renderer, x, y);

					renderer.mesh.SetTile (x - renderer.regionX * Grid.REGION_SIZE, y - renderer.regionY * Grid.REGION_SIZE, info.GetSprite(tile.subId));
					renderer.mesh.ApplyUV ();
					break;
				}
			case TileShape.DOWN_LEFT_TRIANGLE:
			case TileShape.DOWN_RIGHT_TRIANGLE:
			case TileShape.UP_LEFT_TRIANGLE:
			case TileShape.UP_RIGHT_TRIANGLE: {
					PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);

					renderer.mesh.PrepareUV ();
					Clear (renderer, x, y);

					JoinTriangle(renderer, info, tile, x, y);
					renderer.mesh.ApplyUV ();
					break;
				}
			}
		}

		public override void OnHideRegion(int regionX, int regionY) {
			ClearRegionRenderer (regionX, regionY);
		}
		public override void OnShowRegion(int regionX, int regionY) {
			FiniteGrid region = grid.GetRegion (regionX, regionY);
			PositionRegionRenderer renderer = GetRegionRenderer (regionX, regionY);

			renderer.mesh.PrepareUV ();

			for (int i = 0; i < Grid.REGION_SIZE; i++) {
				for (int j = 0; j < Grid.REGION_SIZE; j++) {
					Tile tile = region.Get (i, j);

					if (tile != null && tile.id != 0) {
						renderer.mesh.SetTile (i, j, grid.atlas.GetSprite (tile.id, tile.subId));
					} else {
						renderer.mesh.SetTile (i, j, grid.atlas.emptySprite);
					}
				}
			}
			renderer.mesh.ApplyUV ();
		}

		void Clear(PositionRegionRenderer currentRenderer, int x, int y) {
			GridTriangle triangle = triangles.Get (x, y) as GridTriangle;

			int relX = x - currentRenderer.regionX * Grid.REGION_SIZE;
			int relY = y - currentRenderer.regionY * Grid.REGION_SIZE;

			if (triangle != null) {
				TileInfo info = grid.atlas [triangle.id];
				int actualSize;
					
				if (triangle.isVertical) {
					if (y == triangle.bottomLeftY) {
						if (triangle.height == 1) {
							DestroyImmediate (triangle.gameObject);
						} else {
							triangle.height -= 1;
							triangle.bottomLeftY += 1;

							Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.height);
							int bottomRelY = triangle.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;
							for (int i = 0; i < triangle.height; i++) {
								currentRenderer.mesh.SetPartialVerticalTile (relX, bottomRelY + i, sprite, i % actualSize);
							}
						}
					} else if (y == triangle.bottomLeftY + triangle.height - 1) {
						triangle.height -= 1;

						Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.height);
						int bottomRelY = triangle.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;
						for (int i = 0; i < triangle.height; i++) {
							currentRenderer.mesh.SetPartialVerticalTile (relX, bottomRelY + i, sprite, i % actualSize);
						}
					} else {
						GridTriangle other = GridTriangle.CreateTriangle (
							containerGO, 
							grid,
							triangle.id, triangle.subId, true, 
							triangle.bottomLeftX, y + 1,
							1, triangle.height - (y + 1 - triangle.bottomLeftY)
						);

						triangle.height = y - triangle.bottomLeftY;

						Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.height);
						int bottomRelY = triangle.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;
						for (int i = 0; i < other.height; i++) {
							currentRenderer.mesh.SetPartialVerticalTile (relX, bottomRelY + i, sprite, i % actualSize);
						}

						sprite = info.GetSprite (out actualSize, other.subId, other.height);
						bottomRelY = other.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;
						for (int i = 0; i < other.height; i++) {
							currentRenderer.mesh.SetPartialVerticalTile (relX, bottomRelY + i, sprite, i % actualSize);
							triangles.Set (other.bottomLeftX, other.bottomLeftY + i, other);
						}
					}
				} else {
					if (x == triangle.bottomLeftX) {
						if (triangle.width == 1) {
							DestroyImmediate (triangle.gameObject);
						} else {
							triangle.width -= 1;
							triangle.bottomLeftX += 1;

							Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.width);
							int bottomRelX = triangle.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;
							for (int i = 0; i < triangle.width; i++) {
								currentRenderer.mesh.SetPartialHorizontalTile (bottomRelX + i, relY, sprite, i % actualSize);
							}
						}
					} else if (x == triangle.bottomLeftX + triangle.width - 1) {
						triangle.width -= 1;

						Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.width);
						int bottomRelX = triangle.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;
						for (int i = 0; i < triangle.width; i++) {
							currentRenderer.mesh.SetPartialHorizontalTile (bottomRelX + i, relY, sprite, i % actualSize);
						}
					} else {
						GridTriangle other = GridTriangle.CreateTriangle (
							containerGO, 
							grid,
							triangle.id, triangle.subId, false, 
							x + 1, triangle.bottomLeftY,
							triangle.width - (x + 1 - triangle.bottomLeftX), 1
						);

						triangle.width = x - triangle.bottomLeftX;

						Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.width);
						int bottomRelX = triangle.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;
						for (int i = 0; i < triangle.width; i++) {
							currentRenderer.mesh.SetPartialHorizontalTile (bottomRelX + i, relY, sprite, i % actualSize);
						}

						sprite = info.GetSprite (out actualSize, other.subId, other.width);
						bottomRelX = other.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;
						for (int i = 0; i < other.width; i++) {
							currentRenderer.mesh.SetPartialHorizontalTile (bottomRelX + i, relY, sprite, i % actualSize);
							triangles.Set (other.bottomLeftX + i, other.bottomLeftY, other);
						}
					}
				}

				triangles.Set (x, y, null);
			}

			currentRenderer.mesh.SetTile (x - currentRenderer.regionX * Grid.REGION_SIZE, y - currentRenderer.regionY * Grid.REGION_SIZE, grid.atlas.emptySprite);
		}

		void JoinTriangle(PositionRegionRenderer currentRenderer, TileInfo info, Tile tile, int x, int y) {
			bool isExpanded = false;

			int relX = x - currentRenderer.regionX * Grid.REGION_SIZE;
			int relY = y - currentRenderer.regionY * Grid.REGION_SIZE;

			if (info.isVertical) {
				GridTriangle down = triangles.Get (x, y - 1) as GridTriangle;
				GridTriangle up = triangles.Get (x, y + 1) as GridTriangle;

				if (down != null && down.id == tile.id && down.subId == tile.subId) {
					down.height++;

					triangles.Set (x, y, down);
					isExpanded = true;
				}

				if (up != null && up.id == tile.id && up.subId == tile.subId) {
					if (isExpanded) {
						down.height += up.height;

						for (int i = y + 1; i <= y + up.height; i++) {
							triangles.Set (x, i, down);
						}
					} else {
						up.height++;
						up.bottomLeftY--;

						triangles.Set (x, y, up);
						isExpanded = true;
					}
				}

				if (isExpanded) {
					if (down != null) {
						int actualSize;
						Sprite sprite = info.GetSprite (out actualSize, down.subId, down.height);
						int relBottomY = down.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;

						for (int i = 0; i < down.height; i++) {
							currentRenderer.mesh.SetPartialVerticalTile (relX, relBottomY + i, sprite, i % actualSize);
						}
					} else if (up != null) {
						int actualSize;
						Sprite sprite = info.GetSprite (out actualSize, up.subId, up.height);
						int relBottomY = up.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;

						for (int i = 0; i < up.height; i++) {
							currentRenderer.mesh.SetPartialVerticalTile (relX, relBottomY + i, sprite, i % actualSize);
						}
					}
				}
			} else {
				GridTriangle left = triangles.Get (x - 1, y) as GridTriangle;
				GridTriangle right = triangles.Get (x + 1, y) as GridTriangle;

				if (left != null && left.id == tile.id && left.subId == tile.subId) {
					left.width++;

					triangles.Set (x, y, left);
					isExpanded = true;
				}

				if (right != null && right.id == tile.id && right.subId == tile.subId) {
					if (isExpanded) {
						left.width += right.width;

						for (int i = x + 1; i <= x + right.width; i++) {
							triangles.Set (i, y, left);
						}
					} else {
						right.width++;
						right.bottomLeftX--;

						triangles.Set (x, y, right);
						isExpanded = true;
					}
				}

				if (isExpanded) {
					if (left != null) {
						int actualSize;
						Sprite sprite = info.GetSprite (out actualSize, left.subId, left.width);
						int relBottomX = left.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;

						for (int i = 0; i < left.width; i++) {
							currentRenderer.mesh.SetPartialHorizontalTile (relBottomX + i, relY, sprite, i % actualSize);
						}
					} else if (right != null) {
						int actualSize;
						Sprite sprite = info.GetSprite (out actualSize, right.subId, right.width);
						int relBottomX = right.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;

						for (int i = 0; i < right.width; i++) {
							currentRenderer.mesh.SetPartialHorizontalTile (relBottomX + i, relY, sprite, i % actualSize);
						}
					}
				}
			}

			if (!isExpanded) {
				triangles.Set(x, y, GridTriangle.CreateTriangle(containerGO, grid, tile.id, tile.subId, info.isVertical, x, y, 1, 1));
					
				currentRenderer.mesh.SetTile (relX, relY, info.GetSprite (tile.subId));
			}
		}
	}
}