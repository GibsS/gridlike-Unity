using UnityEngine;
using System;

// TODO
[Serializable]
public class DragTool : GridTool {

	public override bool UseWindow () {
		return false;
	}
	public override string Name() {
		return "drag";
	}
}