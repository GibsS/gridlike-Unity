using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NotificationView : MonoBehaviour {

	const float NOTIFICATION_DURATION = 6;

	public Text text;

	Queue<string> notifications;
	float lastNotificationDate;

	void Initialize() {
		notifications = new Queue<string> ();
	}

	void Update() {
		if (Time.time - lastNotificationDate > NOTIFICATION_DURATION) {
			if (notifications.Count > 0) {
				lastNotificationDate = Time.time;
				gameObject.SetActive (true);
				text.text = notifications.Dequeue ();
			} else {
				gameObject.SetActive (false);
			}
		}
	}

	public void ShowNotification(string notification) {
		notifications.Enqueue(notification);
	}

	public void Show() {
		gameObject.SetActive (true);
	}
	public void Hide() {
		gameObject.SetActive (false);
	}
}
