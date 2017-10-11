using UnityEngine;
using System.Collections;

public class TestGridGeneratorAlgorithm : GridGeneratorAlgorithm {

	// TODO make sure to keep it big enough
	public int width;
	public int height;

	// TODO move this to grid generator
	public override int generationRegionWidth { get { return width; } }
	public override int generationRegionHeight { get { return height; } }

	//bool done = true;

	void Update() {
		if (Input.GetMouseButtonDown (0)) {

			Vector2 mouseInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 mouseInTransform = transform.InverseTransformPoint (mouseInWorld);

			int regionX = Mathf.FloorToInt(mouseInTransform.x / Grid.REGION_SIZE);
			int regionY = Mathf.FloorToInt(mouseInTransform.y / Grid.REGION_SIZE);

			Grid grid = GetComponent<Grid> ();
				
			grid.LoadRegion (regionX, regionY);
		}

		if (Input.GetMouseButton (1)) {
			Vector2 mouseInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 mouseInTransform = transform.InverseTransformPoint (mouseInWorld);

			int x = Mathf.FloorToInt(mouseInTransform.x);
			int y = Mathf.FloorToInt(mouseInTransform.y);

			Grid grid = GetComponent<Grid> ();

			Debug.Log (grid.Get (x, y).id);

			//grid.SetId (x, y, 0, -1);
		}
	}

	public override Tile[,] GenerateTiles (int x, int y, int width, int height) {
		Tile[,] tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile {
					id = (j + y > i + x) ? 0 : 1,
					subId = -1,
					shape = TileShape.FULL
				};
			}
		}

		return tiles;
	}
}

