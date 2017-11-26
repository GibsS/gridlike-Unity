using UnityEngine;
using System.Collections;

public class Tutorial {

	public static Tutorial instance;

	INotifier notifier;

	bool onShipFlag;

	public void _Inject(INotifier notifier) {
		instance = this;
		this.notifier = notifier;
	}

	protected void Notify(string notification) {
		notifier.Notify (notification);
	}

	public void Upgrade() {
		if (!onShipFlag && GSSingleton.instance.ship.hasCharacter) {
			onShipFlag = true;

			Notify ("Hold F, G, H and T to move the ship");
		}	
	}
}