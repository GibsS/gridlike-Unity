using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NotificationView : MonoBehaviour {

	const float NOTIFICATION_DURATION = 2;

	public Text text;
	public GameObject container;

	Queue<string> notifications;
	float lastNotificationDate = -NOTIFICATION_DURATION;

	public void Initialize() {
		notifications = new Queue<string> ();
	}

	void Update() {
		if (Time.time - lastNotificationDate > NOTIFICATION_DURATION) {
			if (notifications.Count > 0) {
				lastNotificationDate = Time.time;
				container.SetActive (true);
				text.text = notifications.Dequeue ();
			} else {
				container.SetActive (false);
			}
		}
	}

	public void ShowNotification(string notification) {
		notifications.Enqueue(notification);
	}

	public void Show() {
		container.SetActive (true);
	}
	public void Hide() {
		container.SetActive (false);
	}
}
