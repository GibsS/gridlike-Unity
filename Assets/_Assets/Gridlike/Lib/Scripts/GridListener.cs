using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {
	
	[ExecuteInEditMode]
	[RequireComponent(typeof(Grid))]
	public abstract class GridListener : MonoBehaviour {
		
		[SerializeField] public Grid grid { get; private set; }

		void Reset() {
			ResetListener ();
		}
		public virtual void Awake() {
			ResetListener ();
		}

		public virtual void OnDestroy() {
			if (this.grid != null) {
				this.grid.RemoveListener (this);
			}
		} 

		public virtual void ResetListener() {
			if (this.grid == null) {
				Grid grid = GetComponent<Grid> ();

				if (grid == null) {
					Debug.LogWarning ("[Gridlike] No Grid on this game object");
				} else {
					this.grid = grid;

					grid.AddListener (this);
				}
			}
		}
			
		public abstract void OnSet(int x, int y, Tile tile);
		public virtual void OnSetState (int x, int y, Tile tile, float oldState1, float oldState2, float oldState3) {
			OnSet (x, y, tile);
		}
		public virtual void OnSetSubId (int x, int y, Tile tile, int oldSubId) {
			OnSet (x, y, tile);
		}

		public virtual void OnSet(int x, int y, int width, int height) {
			int minRegionX = Mathf.FloorToInt (x / (float) Grid.REGION_SIZE);
			int minRegionY = Mathf.FloorToInt (y / (float) Grid.REGION_SIZE);
			int maxRegionX = Mathf.FloorToInt ((x + width) / (float) Grid.REGION_SIZE);
			int maxRegionY = Mathf.FloorToInt ((y + height) / (float) Grid.REGION_SIZE);

			for (int regionX = minRegionX; regionX <= maxRegionX; regionX++) {
				for (int regionY = minRegionY; regionY <= maxRegionY; regionY++) {
					FiniteGrid region = grid.GetRegion (regionX, regionY);

					if (region.presented) {
						int startX = regionX * Grid.REGION_SIZE;
						int startY = regionY * Grid.REGION_SIZE;

						int minX = Mathf.Max (x, startX);
						int minY = Mathf.Max (y, startY);
						int maxX = Mathf.Min (x + width, (regionX + 1) * Grid.REGION_SIZE);
						int maxY = Mathf.Min (y + height, (regionY + 1) * Grid.REGION_SIZE);

						for (int i = minX; i < maxX; i++) {
							for (int j = minY; j < maxY; j++) {
								Tile tile = region.Get (i - startX, j - startY);

								if (tile != null) {
									OnSet (i, j, tile);
								}
							}
						}
					}
				}
			}
		}

		public virtual void OnShowRegion(int regionX, int regionY) {
			FiniteGrid region = grid.GetRegion (regionX, regionY);

			int startX = regionX * Grid.REGION_SIZE;
			int endX = (regionX + 1) * Grid.REGION_SIZE;
			int startY = regionY * Grid.REGION_SIZE;
			int endY = (regionY + 1) * Grid.REGION_SIZE;

			for (int i = startX; i < endX; i++) {
				for(int j = startY; j < endY; j++) {
					Tile tile = region.Get (i - startX, j - startY);

					if(tile != null) OnSet(i, j, tile);
				}
			}
		}

		public abstract void OnHideRegion(int X, int Y);
	}
}