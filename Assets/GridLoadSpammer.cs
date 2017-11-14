using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class GridLoadSpammer : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		GetComponent<Grid> ().LoadRegion (5, 5);
	}
}
