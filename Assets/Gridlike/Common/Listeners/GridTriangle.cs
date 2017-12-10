using UnityEngine;
using System.Collections;

namespace Gridlike {
	
	public class GridTriangle : MonoBehaviour {

		public int id;
		public int subId;
		public bool isVertical;

		public int width;
		public int height;
		public int bottomLeftX;
		public int bottomLeftY; 

		public static GridTriangle CreateTriangle(GameObject parent, Grid grid, int id, int subId, bool vertical, int x, int y, int width, int height) {
			GameObject obj = new GameObject ("sprite x=" + x + " y=" + y);
			obj.transform.SetParent (parent.transform);
			obj.transform.localPosition = grid.TileCenterInTransform (x, y);
			obj.transform.localScale = new Vector3 (1, 1, 1);

			GridTriangle triangle = obj.AddComponent<GridTriangle> ();

			triangle.id = id;
			triangle.subId = subId;
			triangle.isVertical = vertical;

			triangle.width = width;
			triangle.height = height;
			triangle.bottomLeftX = x;
			triangle.bottomLeftY = y;

			return triangle;
		}

		public bool IsCompatible(GridTriangle other) {
			return id == other.id && subId == other.subId;
		}
		public bool IsCompatible(int id, int subId, bool vertical) {
			return this.id == id && this.subId == subId && this.isVertical == vertical;
		}
	}
}