using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantHorizontalMovement : MonoBehaviour {

	[Range(1, 100)]
	public float speed;

	void Update () {
		transform.position = (Vector2)transform.position + Vector2.right * speed * Time.deltaTime;
	}
}
