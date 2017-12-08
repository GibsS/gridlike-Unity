using UnityEngine;
using System.Collections;

using Gridlike;

public class IncreaseJumpHeight : Upgrade {

	float heightIncrease;

	public IncreaseJumpHeight(int heightIncrease) {
		this.heightIncrease = heightIncrease;
	}

	public override string Name() {
		return "Jump height++";
	}
	public override string Description () {
		return "You can jump higher!";
	}

	public override void Execute () {
		character.GetComponent<PlatformerMotor2D>().jumpHeight += heightIncrease;
	}
}