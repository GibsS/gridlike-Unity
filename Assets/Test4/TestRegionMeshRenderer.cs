using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRegionMeshRenderer : MonoBehaviour {

	[SerializeField] RegionMeshRenderer regionRenderer;
	[SerializeField] Texture2D texture;

	[SerializeField] Sprite sprite;

	void Start () {
		regionRenderer.Initialize (50, texture);
	}

	void OnGUI() {
		if (GUI.Button (new Rect (10, 40, 100, 30), "Set tiles (unoptimized)")) {
			TEST2_SetTiles (sprite);
		}
	}
	void TEST2_SetTiles(Sprite sprite) {
		regionRenderer.PrepareUV ();

		for (int i = 1; i < 50 - 1; i++) {
			for (int j = 1; j < 50 - 1; j++) {
				regionRenderer.SetTile (i, j, sprite);
			}
		}

		regionRenderer.ApplyUV ();
	}
}
