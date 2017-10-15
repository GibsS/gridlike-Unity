using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantHorizontalMovement : MonoBehaviour {
	
	void Update () {
		transform.position = (Vector2)transform.position + Vector2.right * 10 * Time.deltaTime;
	}
}
