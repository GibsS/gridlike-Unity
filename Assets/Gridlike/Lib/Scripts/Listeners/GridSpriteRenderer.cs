using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpriteRenderer : GridListener {

	[SerializeField]
	InfiniteComponentGrid components;

	public override void ResetGrid() {
		base.ResetGrid ();

		if (components == null) {
			components = new InfiniteComponentGrid (Grid.REGION_SIZE);
		}
	}

	public override void OnTileChange(int x, int y) {
		SpriteRenderer renderer = components.Get (x, y) as SpriteRenderer;

		if (renderer == null) {
			GameObject obj = new GameObject ("sprite x=" + x + " y=" + y);

			renderer = obj.AddComponent<SpriteRenderer> ();
			renderer.sprite = Resources.Load<Sprite> ("test-block");

			obj.transform.SetParent (transform);
			obj.transform.localPosition = grid.TileCenterInTransform (x, y);

			components.Set (x, y, renderer);
		} 
	}
	public override void OnRegionChange(int regionX, int regionY) {

	}
}