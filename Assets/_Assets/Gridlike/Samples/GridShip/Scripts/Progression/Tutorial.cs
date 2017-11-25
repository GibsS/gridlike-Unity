using UnityEngine;
using System.Collections;

public class Tutorial {

	INotifier notifier;

	public void _Inject(INotifier notifier) {
		this.notifier = notifier;
	}

	protected void Notify(string notification) {
		notifier.Notify (notification);
	}

	public void Upgrade() {

	}
}