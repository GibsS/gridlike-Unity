using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpriteGenerator : MonoBehaviour {

	public int width = 100;
	public int height = 100;

	public Sprite sprite;

	void Start () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject obj = new GameObject ();
				obj.AddComponent<SpriteRenderer> ().sprite = sprite;
				obj.transform.localPosition = new Vector2 (i, j);
			}
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.A)) {
			Start ();
		}
	}
}