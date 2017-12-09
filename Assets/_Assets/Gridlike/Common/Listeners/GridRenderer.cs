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

	/// <summary>
	/// A GridListener that generates meshes for rendering the different tiles.
	/// </summary>
	[AddComponentMenu("Gridlike/Grid renderer")]
	public class GridRenderer : GridListener {

		[HideInInspector] [SerializeField] List<PositionRegionRenderer> regionRenderers;
		[HideInInspector] [SerializeField] InfiniteComponentGrid triangles;

		[HideInInspector] [SerializeField] GameObject containerGO;

		PositionRegionRenderer upRenderer;
		PositionRegionRenderer downRenderer;
		PositionRegionRenderer rightRenderer;
		PositionRegionRenderer leftRenderer;

		public override void OnDestroy() {
			base.OnDestroy ();

			if (Application.isPlaying)
				Destroy (containerGO);
			else
				DestroyImmediate (containerGO);
			containerGO = null;
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

			if (grid.atlas.material == null) {
				grid.atlas.RegenerateMaterial ();
			}
			rend.Initialize (grid.atlas.material, grid.atlas.tilePixelSize, grid.atlas.emptySprite);

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
					rend.mesh.PrepareUV ();

					for (int i = 0; i < Grid.REGION_SIZE; i++) {
						for (int j = 0; j < Grid.REGION_SIZE; j++) {
							rend.mesh.Clear (i, j);
						}
					}

					rend.mesh.ApplyUV ();

					GridlikePools.renderers.Free (rend.mesh);
				} else {
					rend.mesh.Destroy();
				}

				regionRenderers.Remove (rend);
			}
		}

		public override void OnSet(int x, int y, Tile tile) {
			PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);
			GridTriangle triangle = triangles.Get (x, y) as GridTriangle;

			ClearRenderers ();
			renderer.mesh.PrepareUV ();

			_OnSet (triangle, renderer, x, y, tile);

			renderer.mesh.ApplyUV ();
			ApplyRenderers ();
		}
		void _OnSet(GridTriangle triangle, PositionRegionRenderer renderer, int x, int y, Tile tile) {
			TileInfo info = grid.atlas [tile.id];

			Clear (triangle, renderer, x, y);

			switch (info.shape) {
			case TileShape.UP_ONEWAY:
			case TileShape.RIGHT_ONEWAY:
			case TileShape.DOWN_ONEWAY:
			case TileShape.LEFT_ONEWAY:
			case TileShape.FULL: {
					renderer.mesh.SetTile (x - renderer.regionX * Grid.REGION_SIZE, y - renderer.regionY * Grid.REGION_SIZE, info.GetSprite(tile.subId));
					break;
				}
			case TileShape.DOWN_LEFT_TRIANGLE:
			case TileShape.DOWN_RIGHT_TRIANGLE:
			case TileShape.UP_LEFT_TRIANGLE:
			case TileShape.UP_RIGHT_TRIANGLE: {

					JoinTriangle(renderer, info, tile, x, y);
					break;
				}
			}
		}

		public override void OnSet(int x, int y, int width, int height) {
			int minRegionX = Mathf.FloorToInt (x / (float) Grid.REGION_SIZE);
			int minRegionY = Mathf.FloorToInt (y / (float) Grid.REGION_SIZE);
			int maxRegionX = Mathf.FloorToInt ((x + width) / (float) Grid.REGION_SIZE);
			int maxRegionY = Mathf.FloorToInt ((y + height) / (float) Grid.REGION_SIZE);

			for (int regionX = minRegionX; regionX <= maxRegionX; regionX++) {
				for (int regionY = minRegionY; regionY <= maxRegionY; regionY++) {
					FiniteGrid region = grid.GetRegion (regionX, regionY);

					if (region.presented) {
						PositionRegionRenderer renderer = GetRegionRenderer (regionX, regionY);
						FiniteComponentGrid triangleRegion = triangles.GetRegion (regionX, regionY);

						int startX = regionX * Grid.REGION_SIZE;
						int startY = regionY * Grid.REGION_SIZE;

						int minX = Mathf.Max (x, startX);
						int minY = Mathf.Max (y, startY);
						int maxX = Mathf.Min (x + width, (regionX + 1) * Grid.REGION_SIZE);
						int maxY = Mathf.Min (y + height, (regionY + 1) * Grid.REGION_SIZE);

						renderer.mesh.PrepareUV ();

						for (int i = minX; i < maxX; i++) {
							for (int j = minY; j < maxY; j++) {
								Tile tile = region.Get (i - startX, j - startY);

								if (tile != null) {
									if (triangleRegion != null) {
										_OnSet (triangleRegion.Get (x - startX, y - startY) as GridTriangle, renderer, i, j, tile);
									} else {
										_OnSet (null, renderer, i, j, tile);
									}
								} 
							}
						}

						renderer.mesh.ApplyUV ();
					}
				}
			}
		}

		public override void OnHideRegion(int regionX, int regionY) {
			ClearRegionRenderer (regionX, regionY);
			triangles.ClearRegion (regionX, regionY);
		}
		public override void OnShowRegion(int regionX, int regionY) {
			FiniteGrid region = grid.GetRegion (regionX, regionY);
			FiniteComponentGrid regionTriangle = triangles.GetRegion (regionX, regionY);
			PositionRegionRenderer renderer = GetRegionRenderer (regionX, regionY);

			ClearRenderers ();
			renderer.mesh.PrepareUV ();

			int bx = regionX * Grid.REGION_SIZE;
			int by = regionY * Grid.REGION_SIZE;

			for (int i = 0; i < Grid.REGION_SIZE; i++) {
				for (int j = 0; j < Grid.REGION_SIZE; j++) {
					Tile tile = region.Get (i, j);

					if (tile != null && tile.id != 0) {
						if (regionTriangle == null) {
							_OnSet (null, renderer, i + bx, j + by, tile);
						} else {
							_OnSet (regionTriangle.Get (i, j) as GridTriangle, renderer, i + bx, j + by, tile);
						}
					}
				}
			}

			renderer.mesh.ApplyUV ();
			ApplyRenderers ();
		}

		void Clear(GridTriangle triangle, PositionRegionRenderer currentRenderer, int x, int y) {
			int relX = x - currentRenderer.regionX * Grid.REGION_SIZE;
			int relY = y - currentRenderer.regionY * Grid.REGION_SIZE;

			if (triangle != null) {

				TileInfo info = grid.atlas [triangle.id];
				int actualSize;
					
				if (triangle.isVertical) {
					int bottomRelY = triangle.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;

					if (y == triangle.bottomLeftY) {
						if (triangle.height == 1) {
							DestroyImmediate (triangle.gameObject);
						} else {
							triangle.height -= 1;
							triangle.bottomLeftY += 1;

							Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.height);
							RenderUpTriangle (currentRenderer, relX, bottomRelY + 1, triangle.height, actualSize, sprite);
						}
					} else if (y == triangle.bottomLeftY + triangle.height - 1) {
						triangle.height -= 1;

						Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.height);
						RenderDownTriangle (currentRenderer, relX, bottomRelY, triangle.height, actualSize, sprite);
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
						RenderDownTriangle (currentRenderer, relX, bottomRelY, triangle.height, actualSize, sprite);

						sprite = info.GetSprite (out actualSize, other.subId, other.height);
						bottomRelY = other.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE;
						for (int i = 0; i < other.height; i++) {
							triangles.Set (other.bottomLeftX, other.bottomLeftY + i, other);
						}
						RenderUpTriangle (currentRenderer, relX, bottomRelY, other.height, actualSize, sprite);
					}
				} else {
					int bottomRelX = triangle.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;

					if (x == triangle.bottomLeftX) {
						if (triangle.width == 1) {
							DestroyImmediate (triangle.gameObject);
						} else {
							triangle.width -= 1;
							triangle.bottomLeftX += 1;

							Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.width);
							RenderRightTriangle (currentRenderer, bottomRelX + 1, relY, triangle.width, actualSize, sprite);
						}
					} else if (x == triangle.bottomLeftX + triangle.width - 1) {
						triangle.width -= 1;

						Sprite sprite = info.GetSprite (out actualSize, triangle.subId, triangle.width);
						RenderLeftTriangle (currentRenderer, bottomRelX, relY, triangle.width, actualSize, sprite);
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
						RenderLeftTriangle (currentRenderer, bottomRelX, relY, triangle.width, actualSize, sprite);

						sprite = info.GetSprite (out actualSize, other.subId, other.width);
						bottomRelX = other.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;
						for (int i = 0; i < other.width; i++) {
							triangles.Set (other.bottomLeftX + i, other.bottomLeftY, other);
						}
						RenderRightTriangle (currentRenderer, bottomRelX, relY, other.width, actualSize, sprite);
					}
				}

				triangles.Set (x, y, null);
			}

			currentRenderer.mesh.Clear (relX, relY);
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

						if (relBottomY > Grid.REGION_SIZE / 2) {
							RenderUpTriangle (currentRenderer, relX, relBottomY, down.height, actualSize, sprite);
						} else {
							RenderDownTriangle (currentRenderer, relX, relBottomY, down.height, actualSize, sprite);
						}
					} else if (up != null) {
						int actualSize;
						Sprite sprite = info.GetSprite (out actualSize, up.subId, up.height);
						int relBottomY = up.bottomLeftY - currentRenderer.regionY * Grid.REGION_SIZE; 

						if (relBottomY > Grid.REGION_SIZE / 2) {
							RenderUpTriangle (currentRenderer, relX, relBottomY, up.height, actualSize, sprite);
						} else {
							RenderDownTriangle (currentRenderer, relX, relBottomY, up.height, actualSize, sprite);
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

						if (relBottomX > Grid.REGION_SIZE / 2) {
							RenderRightTriangle (currentRenderer, relBottomX, relY, left.width, actualSize, sprite);
						} else {
							RenderLeftTriangle (currentRenderer, relBottomX, relY, left.width, actualSize, sprite);
						}
					} else if (right != null) {
						int actualSize;
						Sprite sprite = info.GetSprite (out actualSize, right.subId, right.width);
						int relBottomX = right.bottomLeftX - currentRenderer.regionX * Grid.REGION_SIZE;

						if (relBottomX > Grid.REGION_SIZE / 2) {
							RenderRightTriangle (currentRenderer, relBottomX, relY, right.width, actualSize, sprite);
						} else {
							RenderLeftTriangle (currentRenderer, relBottomX, relY, right.width, actualSize, sprite);
						}
					}
				}
			}

			if (!isExpanded) {
				triangles.Set(x, y, GridTriangle.CreateTriangle(containerGO, grid, tile.id, tile.subId, info.isVertical, x, y, 1, 1));
					
				currentRenderer.mesh.SetTile (relX, relY, info.GetSprite (tile.subId));
			}
		}

		void RenderUpTriangle(PositionRegionRenderer renderer, int relX, int relY, int height, int actualSize, Sprite sprite) {
			if (relY + height > Grid.REGION_SIZE) {
				int count = 0;

				for (int i = relY; i < Grid.REGION_SIZE; i++, count++) {
					renderer.mesh.SetPartialVerticalTile (relX, i, sprite, count % actualSize);
				}

				if (upRenderer == null) {
					upRenderer = GetRegionRenderer (renderer.regionX, renderer.regionY + 1);
					upRenderer.mesh.PrepareUV ();
				}
				for (int i = 0; i < relY + height - Grid.REGION_SIZE; i++, count++) {
					upRenderer.mesh.SetPartialVerticalTile (relX, i, sprite, count % actualSize);
				}
			} else {
				for (int i = 0; i < height; i++) {
					renderer.mesh.SetPartialVerticalTile (relX, relY + i, sprite, i % actualSize);
				}
			}
		}
		void RenderLeftTriangle(PositionRegionRenderer renderer, int relX, int relY, int width, int actualSize, Sprite sprite) {
			if (relX < 0) {
				int count = 0;

				if (leftRenderer == null) {
					leftRenderer = GetRegionRenderer (renderer.regionX - 1, renderer.regionY);
					leftRenderer.mesh.PrepareUV ();
				}
				for (int i = relX + Grid.REGION_SIZE; i < Grid.REGION_SIZE; i++, count++) {
					leftRenderer.mesh.SetPartialHorizontalTile (i, relY, sprite, count % actualSize);
				}

				for (int i = 0; i < relX + width; i++, count++) {
					renderer.mesh.SetPartialHorizontalTile (i, relY, sprite, count % actualSize);
				}
			} else {
				for (int i = 0; i < width; i++) {
					renderer.mesh.SetPartialHorizontalTile (relX + i, relY, sprite, i % actualSize);
				}
			}
		}		
		void RenderRightTriangle(PositionRegionRenderer renderer, int relX, int relY, int width, int actualSize, Sprite sprite) {
			if (relX + width > Grid.REGION_SIZE) {
				int count = 0;

				for (int i = relX; i < Grid.REGION_SIZE; i++, count++) {
					renderer.mesh.SetPartialHorizontalTile (i, relY, sprite, count % actualSize);
				}

				if (rightRenderer == null) {
					rightRenderer = GetRegionRenderer (renderer.regionX + 1, renderer.regionY);
					rightRenderer.mesh.PrepareUV ();
				}
				for (int i = 0; i < relX + width - Grid.REGION_SIZE; i++, count++) {
					rightRenderer.mesh.SetPartialHorizontalTile (i, relY, sprite, count % actualSize);
				}
			} else {
				for (int i = 0; i < width; i++) {
					renderer.mesh.SetPartialHorizontalTile (relX + i, relY, sprite, i % actualSize);
				}
			}
		}
		void RenderDownTriangle(PositionRegionRenderer renderer, int relX, int relY, int height, int actualSize, Sprite sprite) {
			if (relY < 0) {
				int count = 0;

				if (downRenderer == null) {
					downRenderer = GetRegionRenderer (renderer.regionX, renderer.regionY - 1);
					downRenderer.mesh.PrepareUV ();
				}
				for (int i = relY + Grid.REGION_SIZE; i < Grid.REGION_SIZE; i++, count++) {
					downRenderer.mesh.SetPartialVerticalTile (relX, i, sprite, count % actualSize);
				}

				for (int i = 0; i < relY + height; i++, count++) {
					renderer.mesh.SetPartialVerticalTile (relX, i, sprite, count % actualSize);
				}
			} else {
				for (int i = 0; i < height; i++) {
					renderer.mesh.SetPartialVerticalTile (relX, relY + i, sprite, i % actualSize);
				}
			}
		}

		void ClearRenderers() {
			upRenderer = null;
			downRenderer = null;
			leftRenderer = null;
			rightRenderer = null;
		}

		void ApplyRenderers() {
			if (upRenderer != null) {
				upRenderer.mesh.ApplyUV ();
				upRenderer = null;
			}

			if (downRenderer != null) {
				downRenderer.mesh.ApplyUV ();
				downRenderer = null;
			}

			if (leftRenderer != null) {
				leftRenderer.mesh.ApplyUV ();
				leftRenderer = null;
			}

			if (rightRenderer != null) {
				rightRenderer.mesh.ApplyUV ();
				rightRenderer = null;
			}
		}
	}
}