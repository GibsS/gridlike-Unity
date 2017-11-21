using UnityEngine;
using System.Collections;

public class PrefabStore : MonoBehaviour {

	public static PrefabStore store;

	void Awake() {
		if (store != null) {
			Destroy (this);
			return;
		}

		store = this;
	}

	public GameObject arrow;
	public GameObject cubeParticle;
}