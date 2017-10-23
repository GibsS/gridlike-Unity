using UnityEngine;
using System.Collections;

// TODO
public class EraseRegionTool : GridTool {

	public override bool UseWindow () {
		return true;
	}
	public override string Name() {
		return "erase region";
	}
}