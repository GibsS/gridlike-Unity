using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRegionMeshRenderer : MonoBehaviour {

	[SerializeField] RegionMeshRenderer regionRenderer;
	[SerializeField] Texture2D texture;

	void Start () {
		regionRenderer.Initialize (50, texture);
		//regionRenderer.TEST_SetTiles ();
	}

	void OnGUI() {
		if (GUI.Button (new Rect (10, 10, 100, 30), "Set tiles")) {
			regionRenderer.TEST_SetTiles ();
		}
		if (GUI.Button (new Rect (10, 40, 100, 30), "Set tiles (unoptimized)")) {
			regionRenderer.TEST_SetTiles ();
		}
	}
}
