using UnityEngine;
using System.Collections;

public class GridColliderPart : MonoBehaviour {

	public TileShape shape;
	public int id;
	public bool isVertical;

	public int bottomLeftX;
	public int bottomLeftY;
	public int width;
	public int height;

	public BoxCollider2D box;
	public PolygonCollider2D triangle;
	public EdgeCollider2D line;

	public static GridColliderPart CreateColliderPart(GameObject parent, Grid grid, TileInfo info, int x, int y, int w, int h) {

		switch (info.shape) {
		case TileShape.FULL:
			return BoxGridColliderPart (parent, grid, info, x, y, w, h);
		case TileShape.DOWN_LEFT_TRIANGLE:
		case TileShape.DOWN_RIGHT_TRIANGLE:
		case TileShape.UP_LEFT_TRIANGLE:
		case TileShape.UP_RIGHT_TRIANGLE:
			return TriangleGridColliderPart (parent, grid, info, x, y, w, h);
		case TileShape.DOWN_ONEWAY:
		case TileShape.UP_ONEWAY:
		case TileShape.LEFT_ONEWAY:
		case TileShape.RIGHT_ONEWAY:
			return LineGridColliderPart (parent, grid, info, x, y, w, h);
		default: return null;
		}
	}

	public static GridColliderPart BoxGridColliderPart(GameObject parent, Grid grid, TileInfo tile, int x, int y, int w, int h) {
		GameObject obj = new GameObject ("box");
		obj.transform.SetParent (parent.transform);

		obj.tag = tile.tag;
		obj.layer = tile.layer;

		GridColliderPart part = obj.AddComponent<GridColliderPart> ();
		part.box = obj.AddComponent<BoxCollider2D> ();

		part.shape = TileShape.FULL;
		part.id = tile.id;
		part.box.isTrigger = tile.isSensor;

		part.bottomLeftX = x;
		part.bottomLeftY = y;

		part.transform.localPosition = grid.TileSpaceToTransform (x + w/2f, y + h/2f);

		part.SetSize (w, h);

		return part;
	}

	public static GridColliderPart TriangleGridColliderPart(GameObject parent, Grid grid, TileInfo tile, int x, int y, int w, int h) {
		GameObject obj = new GameObject ("triangle");
		obj.transform.SetParent (parent.transform);

		obj.tag = tile.tag;
		obj.layer = tile.layer;

		GridColliderPart part = obj.AddComponent<GridColliderPart> ();
		part.triangle = obj.AddComponent<PolygonCollider2D> ();

		part.shape = tile.shape;
		part.isVertical = tile.isVertical;

		part.id = tile.id;
		part.triangle.isTrigger = tile.isSensor;

		part.bottomLeftX = x;
		part.bottomLeftY = y;

		part.transform.localPosition = grid.TileSpaceToTransform (x + w/2f, y + h/2f);

		part.SetSize (w, h);

		return part;
	}

	public static GridColliderPart LineGridColliderPart(GameObject parent, Grid grid, TileInfo tile, int x, int y, int w, int h) {
		GameObject obj = new GameObject ("line");
		obj.transform.SetParent (parent.transform);

		obj.tag = tile.tag;
		obj.layer = tile.layer;

		GridColliderPart part = obj.AddComponent<GridColliderPart> ();
		part.line = obj.AddComponent<EdgeCollider2D> ();
		part.line.usedByEffector = true;
		switch (tile.shape) {
		case TileShape.DOWN_ONEWAY: part.transform.Rotate (Vector3.forward * 180); break;
		case TileShape.LEFT_ONEWAY: part.transform.Rotate (Vector3.forward * 90); break;
		case TileShape.RIGHT_ONEWAY: part.transform.Rotate (-Vector3.forward * 90); break;
		}

		PlatformEffector2D effector = part.gameObject.AddComponent<PlatformEffector2D> ();
		effector.surfaceArc = 5;

		part.shape = tile.shape;
		part.id = tile.id;
		part.line.isTrigger = tile.isSensor;

		part.bottomLeftX = x;
		part.bottomLeftY = y;

		part.transform.localPosition = grid.TileSpaceToTransform (x + w/2f, y + h/2f);

		part.SetSize (w, h);

		return part;
	}

	public void SetSize(int w, int h) {
		width = w;
		height = h;

		switch (shape) {
		case TileShape.FULL: {
				box.size = new Vector2 (w, h);
				break;
			}
		case TileShape.DOWN_LEFT_TRIANGLE: {
				triangle.points = new Vector2[] { 
					new Vector2 (-w / 2f, -h / 2f),
					new Vector2 (-w / 2f, h / 2f),
					new Vector2 (w / 2f, -h / 2f)
				};
				break;
			}
		case TileShape.DOWN_RIGHT_TRIANGLE: {
				triangle.points = new Vector2[] { 
					new Vector2 (-w / 2f, -h / 2f),
					new Vector2 (w / 2f, h / 2f),
					new Vector2 (w / 2f, -h / 2f)
				};
				break;
			}
		case TileShape.UP_LEFT_TRIANGLE: {
				triangle.points = new Vector2[] { 
					new Vector2 (-w / 2f, -h / 2f),
					new Vector2 (-w / 2f, h / 2f),
					new Vector2 (w / 2f, h / 2f)
				};
				break;
			}
		case TileShape.UP_RIGHT_TRIANGLE: {
				triangle.points = new Vector2[] { 
					new Vector2 (-w / 2f, h / 2f),
					new Vector2 (w / 2f, h / 2f),
					new Vector2 (w / 2f, -h / 2f)
				};
				break;
			}
		case TileShape.UP_ONEWAY:
		case TileShape.DOWN_ONEWAY: line.points = new Vector2[] { new Vector2 (-w / 2f, h / 2f), new Vector2 (w / 2f, h / 2f) }; break;
		case TileShape.LEFT_ONEWAY:
		case TileShape.RIGHT_ONEWAY: line.points = new Vector2[] { new Vector2 (-h / 2f, w / 2f), new Vector2 (h / 2f, w / 2f) }; break;
		}
	}

	public bool Compatible(GridColliderPart other) {
		return shape == other.shape 
			&& (bottomLeftX != other.bottomLeftX && height == 1 && other.height == 1 
				|| bottomLeftY != other.bottomLeftY && width == 1 && other.width == 1);
	}
	public bool Compatible(TileInfo info) {
		Collider2D collider = (box == null ? (triangle == null ? (line as Collider2D) : (triangle as Collider2D)) : (box as Collider2D));
		return shape == info.shape && gameObject.tag == info.tag && gameObject.layer == info.layer && collider.isTrigger == info.isSensor;
	}
}