public class IncreaseBowDamage : Upgrade {

	int damage;

	public IncreaseBowDamage(int damage) {
		this.damage = damage;
	}

	public override string Name() {
		return "Bow damage++";
	}
	public override string Description () {
		return "Bow damage increased to " + character.bow.damage;
	}

	public override void Execute () {
		character.bow.damage += damage;
	}
}
