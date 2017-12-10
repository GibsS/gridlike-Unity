namespace Gridship {

	public class IncreasePickaxeDamage : Upgrade {

		int damage;

		public IncreasePickaxeDamage(int damage) {
			this.damage = damage;
		}

		public override string Name() {
			return "Pickaxe damage++";
		}
		public override string Description () {
			return "Pickaxe damage increased to " + character.pickaxe.damage;
		}

		public override void Execute () {
			character.pickaxe.damage += damage;
		}
	}
}