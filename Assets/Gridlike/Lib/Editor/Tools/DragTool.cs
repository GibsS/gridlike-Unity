using UnityEngine;
using System.Collections;

// TODO
public class DragTool : GridTool {

	public override bool UseWindow () {
		return false;
	}
	public override string Name() {
		return "drag";
	}
}