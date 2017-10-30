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

		[HideInInspector]
		[SerializeField]
		List<PositionRegionRenderer> components;

		[HideInInspector]
		[SerializeField] 
		GameObject containerGO;

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
			if (components == null) {
				components = new List<PositionRegionRenderer> ();
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
			rend.Initialize (grid.atlas.spriteSheet);

			return rend;
		}

		PositionRegionRenderer GetContainingRegionRenderer(int x, int y) {
			return GetRegionRenderer (Mathf.FloorToInt(((float)x) / Grid.REGION_SIZE), Mathf.FloorToInt(((float)y) / Grid.REGION_SIZE));
		}
		PositionRegionRenderer GetRegionRenderer(int regionX, int regionY) {
			var rend = components.Find (e => e.regionX == regionX && e.regionY == regionY);

			if (rend == null) {
				RegionMeshRenderer regionRenderer = CreateRegionRenderer ();

				regionRenderer.transform.SetParent (containerGO.transform);
				regionRenderer.transform.localPosition = new Vector2 (regionX * Grid.REGION_SIZE, regionY * Grid.REGION_SIZE);

				rend = new PositionRegionRenderer {
					regionX = regionX,
					regionY = regionY,
					mesh = regionRenderer
				};

				components.Add (rend);

				return rend;
			} else {
				return rend;
			}
		}
		void ClearRegionRenderer(int regionX, int regionY) {
			var rend = components.Find (e => e.regionX == regionX && e.regionY == regionY);

			if (rend != null) {
				if (Application.isPlaying) {
					meshes.Free (rend.mesh);
				} else {
					rend.mesh.Destroy();
				}

				components.Remove (rend);
			}
		}

		public override void OnSet(int x, int y, Tile tile) {
			switch (grid.atlas[tile.id].shape) {
			case TileShape.EMPTY: {
					Clear (x, y);
					break;
				}
			case TileShape.UP_ONEWAY:
			case TileShape.RIGHT_ONEWAY:
			case TileShape.DOWN_ONEWAY:
			case TileShape.LEFT_ONEWAY:
			case TileShape.FULL: {
					PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);

					renderer.mesh.PrepareUV ();
					renderer.mesh.SetTile (x - renderer.regionX * Grid.REGION_SIZE, y - renderer.regionY * Grid.REGION_SIZE, grid.atlas.GetSprite(tile.id, tile.subId));
					renderer.mesh.ApplyUV ();
					break;
				}
			case TileShape.DOWN_LEFT_TRIANGLE:
			case TileShape.DOWN_RIGHT_TRIANGLE:
			case TileShape.UP_LEFT_TRIANGLE:
			case TileShape.UP_RIGHT_TRIANGLE: {
					PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);

					renderer.mesh.PrepareUV ();
					renderer.mesh.SetTile (x - renderer.regionX * Grid.REGION_SIZE, y - renderer.regionY * Grid.REGION_SIZE, grid.atlas.GetSprite(tile.id, tile.subId));
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

		void Clear(int x, int y) {
			PositionRegionRenderer renderer = GetContainingRegionRenderer (x, y);

			renderer.mesh.PrepareUV ();
			renderer.mesh.SetTile (x - renderer.regionX * Grid.REGION_SIZE, y - renderer.regionY * Grid.REGION_SIZE, grid.atlas.emptySprite);
			renderer.mesh.ApplyUV ();
		}
	}
}