using UnityEngine;
using System.Collections;

// TODO
// TODO [Tool] Modifier of dictionary fields (maybe in inspector?)  
public class InspectorTool : GridTool {

	public override bool UseWindow () {
		return true;
	}
	public override string Name() {
		return "inspect";
	}
}