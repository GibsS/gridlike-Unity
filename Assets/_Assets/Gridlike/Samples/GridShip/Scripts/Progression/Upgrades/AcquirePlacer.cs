using UnityEngine;
using System.Collections;

public class AcquirePlacer : Upgrade {
	
	public override string Name () {
		return "materializer";
	}

	public override string Description () {
		return "you've acquired the materializer";
	}

	public override void Execute () {
		character.AcquirePlacer ();
	}
}