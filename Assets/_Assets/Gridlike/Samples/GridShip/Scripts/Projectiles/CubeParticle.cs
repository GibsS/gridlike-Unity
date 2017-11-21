using UnityEngine;
using System.Collections;

public class CubeParticle : MonoBehaviour {

	GSCharacter target;

	Vector2 speed;
	float startTime;

	public static void CreateParticles(Vector2 position, GSCharacter character, int count) {
		for (int i = 0; i < count; i++) CreateParticle (position, character);
	}
	public static void CreateParticle(Vector2 position, GSCharacter character) {
		GameObject go = Instantiate (PrefabStore.store.cubeParticle);
		go.GetComponent<CubeParticle> ().Initialize (character);
		go.transform.position = position;
	}

	public void Initialize(GSCharacter character) {
		target = character;
		startTime = Time.time;
		speed = 5 * Random.insideUnitCircle;
	}

	void FixedUpdate() {
		transform.position += (Vector3) speed * Time.fixedDeltaTime;

		Vector2 directionUnit = (target.transform.position - transform.position).normalized;

		speed += Mathf.Min(5, Time.time - startTime) * directionUnit;

		speed = speed * 0.8f + speed.magnitude * directionUnit * 0.2f;

		if (Vector2.Distance (transform.position, target.transform.position) < 0.5f) {
			StartCoroutine (DestroyAnimation ());

			target.AddCubes (1);
		}
	}

	IEnumerator DestroyAnimation() {
		yield return null;

		Destroy (gameObject);
	}
}