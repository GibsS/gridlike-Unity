using UnityEngine;
using System.Collections;

public class GridColliderPart : MonoBehaviour {

	public int bottomLeftX;
	public int bottomLeftY;
	public int width;
	public int height;
	public BoxCollider2D box;

	public static GridColliderPart BoxGridColliderPart(GameObject parent, Grid grid, int x, int y, int w, int h) {
		GameObject obj = new GameObject ("box");
		obj.transform.SetParent (parent.transform);

		GridColliderPart part = obj.AddComponent<GridColliderPart> ();
		part.box = obj.AddComponent<BoxCollider2D> ();

		part.bottomLeftX = x;
		part.bottomLeftY = y;
		part.width = w;
		part.height = h;

		part.transform.localPosition = grid.TileSpaceToTransform (x + w/2f, y + h/2f);
		part.box.size = new Vector2 (w, h);

		return part;
	}
}

