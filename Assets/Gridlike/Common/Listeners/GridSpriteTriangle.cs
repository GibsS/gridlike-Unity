using UnityEngine;
using System.Collections;

namespace Gridlike {
	
	public class GridSpriteTriangle : MonoBehaviour {

		public TileShape shape;

		public int width;
		public int height;
		public int bottomLeftX;
		public int bottomLeftY;

		public SpriteRenderer spriteRenderer;

		public static GridSpriteTriangle CreateSpriteTriangle(GameObject parent, Grid grid, int x, int y) {
			GameObject obj = new GameObject ("sprite x=" + x + " y=" + y);

			GridSpriteTriangle triangle = obj.AddComponent<GridSpriteTriangle> ();

			triangle.spriteRenderer = obj.AddComponent<SpriteRenderer> ();
			triangle.width = 1;
			triangle.height = 1;
			triangle.bottomLeftX = x;
			triangle.bottomLeftY = y;

			obj.transform.SetParent (parent.transform);
			obj.transform.localPosition = grid.TileCenterInTransform (x, y);
			obj.transform.localScale = new Vector3 (1, 1, 1);

			return triangle;
		}

		public void UpdateSprite(Grid grid, Tile tile) {
			spriteRenderer.sprite = grid.atlas.GetSprite (tile.id, tile.subId, 1);
		}
	}
}