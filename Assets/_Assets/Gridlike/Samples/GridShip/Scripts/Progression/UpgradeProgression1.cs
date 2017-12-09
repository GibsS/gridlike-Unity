using UnityEngine;
using System.Collections;

namespace Gridship {
		
	public class UpgradeProgression1 : UpgradeProgression {
		
		AcquireBow bow;
		AcquirePlacer placer;
		ArrowsAreBombs arrowsAreBombs;

		public override void ProgressionDefinition() {
			// SPECIALS
			bow = Add (new AcquireBow ()) as AcquireBow;
			bow.isSpecial = true;

			placer = Add (new AcquirePlacer ()) as AcquirePlacer;
			placer.DependOn (bow);
			placer.isSpecial = true;

			arrowsAreBombs = Add (new ArrowsAreBombs (2)) as ArrowsAreBombs;
			arrowsAreBombs.DependOn (bow).DependOn(placer);
			arrowsAreBombs.isSpecial = true;

			// BOW UPGRADES
			int DAMAGE_INCREASE = 1;
			Upgrade upgrade = AddIncreaseBowDamage(null, DAMAGE_INCREASE);
			upgrade = AddIncreaseBowDamage (upgrade, DAMAGE_INCREASE);
			upgrade = AddIncreaseBowDamage (upgrade, DAMAGE_INCREASE);
			upgrade = AddIncreaseBowDamage (upgrade, DAMAGE_INCREASE);
			upgrade = AddIncreaseBowDamage (upgrade, DAMAGE_INCREASE);

			int RADIUS_INCREASE = 1;
			upgrade = AddIncreaseBowRadius (null, RADIUS_INCREASE);
			upgrade = AddIncreaseBowRadius (upgrade, RADIUS_INCREASE);
			upgrade = AddIncreaseBowRadius (upgrade, RADIUS_INCREASE);
			upgrade = AddIncreaseBowRadius (upgrade, RADIUS_INCREASE);
			upgrade = AddIncreaseBowRadius (upgrade, RADIUS_INCREASE);

			// PICKAXE UPGRADE
			upgrade = Add(new IncreasePickaxeDamage(DAMAGE_INCREASE));
			upgrade = Add(new IncreasePickaxeDamage(DAMAGE_INCREASE), upgrade);
			upgrade = Add(new IncreasePickaxeDamage(DAMAGE_INCREASE), upgrade);

			// MOVEMENT
			Add(new AcquireDoubleJump());

			int SPEED_INCREASE = 1;
			upgrade = Add (new IncreaseWalkSpeed (SPEED_INCREASE));
			upgrade = Add (new IncreaseWalkSpeed (SPEED_INCREASE), upgrade);

			upgrade = Add (new IncreaseJumpHeight (1));
			upgrade = Add (new IncreaseJumpHeight (1), upgrade);
			upgrade = Add (new IncreaseJumpHeight (1), upgrade);

			// GRID ITEM
			AddAcquireGridItem (GSConsts.ENGINE);
			AddAcquireGridItem (GSConsts.ANTI_GRAVITY);
			AddAcquireGridItem (GSConsts.ONE_WAY_PLATFORM);
		}

		Upgrade AddIncreaseBowRadius(Upgrade previous, int radius) {
			Upgrade upgrade = Add (new IncreaseBowRadius (radius));
			upgrade.DependOn (arrowsAreBombs).DependOn (bow);

			if(previous != null) upgrade.DependOn(previous);

			return upgrade;
		}

		Upgrade AddIncreaseBowDamage(Upgrade previous, int damage) {
			Upgrade upgrade = Add (new IncreaseBowDamage (damage));
			upgrade.DependOn (bow);

			if(previous != null) upgrade.DependOn(previous);

			return upgrade;
		}
		Upgrade AddAcquireGridItem(int id) {
			Upgrade upgrade = Add (new AcquireGridItem (id));
			upgrade.DependOn (placer);

			return upgrade;
		}
	}
}