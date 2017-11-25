using UnityEngine;
using System.Collections;

public class ArrowsAreBombs : Upgrade {

	int radius;

	public ArrowsAreBombs(int radius) {
		this.radius = radius;
	}

	public override string Name () {
		return "arrows are bombs";
	}

	public override string Description () {
		return "you're arrows will now explode";
	}

	public override void Execute () {
		character.bow.radius = radius;
	}
}