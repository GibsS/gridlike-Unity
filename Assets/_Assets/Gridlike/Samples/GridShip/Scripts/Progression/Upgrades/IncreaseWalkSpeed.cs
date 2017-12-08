using UnityEngine;
using System.Collections;

using Gridlike;

public class IncreaseWalkSpeed : Upgrade {

	float speedIncrease;

	public IncreaseWalkSpeed(int speedIncrease) {
		this.speedIncrease = speedIncrease;
	}

	public override string Name() {
		return "Speed++";
	}
	public override string Description () {
		return "You can run faster!";
	}

	public override void Execute () {
		character.GetComponent<PlatformerMotor2D>().groundSpeed += speedIncrease;
	}
}