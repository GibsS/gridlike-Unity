using UnityEngine;
using System.Collections;

public class GridClickLoader : MonoBehaviour {

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 300, 40), "Left click: load region. Right click: save region (unloads at the save time).");
	}

	void Update() {
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) {
			Vector2 mouseInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 mouseInTransform = transform.InverseTransformPoint (mouseInWorld);

			int regionX = Mathf.FloorToInt(mouseInTransform.x / Grid.REGION_SIZE);
			int regionY = Mathf.FloorToInt(mouseInTransform.y / Grid.REGION_SIZE);

			Grid grid = GetComponent<Grid> ();

			if (Input.GetMouseButtonDown (0)) {
				grid.LoadRegion (regionX, regionY);
			} else {
				grid.SaveRegion (regionX, regionY, true);
			}
		}
	}
}