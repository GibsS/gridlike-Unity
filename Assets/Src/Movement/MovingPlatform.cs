using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	Rigidbody2D body;

	void Start() {
		body = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {
		body.velocity = new Vector2 (5 * Mathf.Sin (Time.time), 0);
	}
}
