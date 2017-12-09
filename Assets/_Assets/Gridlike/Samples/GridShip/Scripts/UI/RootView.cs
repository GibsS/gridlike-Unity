using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridship {

	public class RootView : MonoBehaviour {

		public NotificationView notifications;
		public TilePickerView tilePicker;
		public ToolbarView toolbar;
		public UpgradePickerView upgrades;
		public StatusView status;

		public void Initialize() {
			notifications.Initialize ();
			toolbar.Initialize ();
			status.Initialize ();
		}

		public void Show() {
			gameObject.SetActive (true);
		}
		public void Hide() {
			gameObject.SetActive (false);
		}
	}
}