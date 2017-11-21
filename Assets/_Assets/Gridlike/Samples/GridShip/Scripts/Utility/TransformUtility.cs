using UnityEngine;
using System.Collections;

public class TransformUtility {

	public static GameObject GetTopParent(GameObject go) {
		Transform transform = go.transform;

		while (transform.parent != null) {
			transform = transform.parent;
		}

		return transform.gameObject;
	}
}

