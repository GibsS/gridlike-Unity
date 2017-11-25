using UnityEngine;
using System.Collections;

public class UpgradeProgression1 : UpgradeProgression {

	public override void ProgressionDefinition() {
		Add (new IncreaseBowDamage (5));
		Add (new IncreasePickaxeDamage (5));
	}
}