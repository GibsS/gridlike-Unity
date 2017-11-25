using UnityEngine;
using System.Collections;

public class IncreaseBowRadius : Upgrade {

	int radius;

	public IncreaseBowRadius(int radius) {
		this.radius = radius;
	}

	public override string Name() {
		return "Bow explosion++";
	}
	public override string Description () {
		return "Bow radius increased to " + character.bow.radius;
	}

	public override void Execute () {
		character.bow.radius += radius;
	}
}