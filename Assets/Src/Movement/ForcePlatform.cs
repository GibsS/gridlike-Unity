using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePlatform : MonoBehaviour {

	public Rigidbody2D rigid;

	public float force;

	void Update() {
		if (Input.GetKey (KeyCode.F) && !Input.GetKey (KeyCode.H)) {
			rigid.AddForce (-force * Vector2.right);
		}
		if (Input.GetKey (KeyCode.H) && !Input.GetKey (KeyCode.F)) {
			rigid.AddForce (force * Vector2.right);
		}
		if (Input.GetKey (KeyCode.T) && !Input.GetKey (KeyCode.G)) {
			rigid.AddForce (force * Vector2.up);
		}
		if (Input.GetKey (KeyCode.G) && !Input.GetKey (KeyCode.T)) {
			rigid.AddForce (-force * Vector2.up);
		}
	}
}
