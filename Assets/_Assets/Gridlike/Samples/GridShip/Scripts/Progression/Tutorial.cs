using UnityEngine;
using System.Collections;

public class Tutorial {

	public static Tutorial instance;

	INotifier notifier;

	public void _Inject(INotifier notifier) {
		instance = this;
		this.notifier = notifier;
	}

	protected void Notify(string notification) {
		notifier.Notify (notification);
	}

	public void Upgrade() {

	}
}