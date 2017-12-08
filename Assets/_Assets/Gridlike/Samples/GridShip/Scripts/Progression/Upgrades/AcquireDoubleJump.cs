using UnityEngine;
using System.Collections;

using Gridlike;

public class AcquireDoubleJump : Upgrade {

	public override string Name () {
		return "double jump";
	}

	public override string Description () {
		return "you've acquired the legendary double jump";
	}

	public override void Execute () {
		character.GetComponent<PlatformerMotor2D> ().numOfAirJumps = 1;
	}
}