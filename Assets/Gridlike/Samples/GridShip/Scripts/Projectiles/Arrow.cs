using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

namespace Gridship {

	public class Arrow : MonoBehaviour {

		GSCharacter character;
		int damage;
		int radius;

		public void Initialize(GSCharacter character, int damage, Vector2 position, Vector2 velocity, int radius = 0) {
			this.character = character;
			this.damage = damage;
			this.radius = radius;

			transform.position = position;

			GetComponent<Rigidbody2D> ().velocity = velocity;

			StartCoroutine (DestroyAfterTime (10));
		}

		void OnCollisionEnter2D(Collision2D collision) {
			if (radius == 0) {
				GameObject go = TransformUtility.GetTopParent (collision.gameObject);

				Grid grid = go.GetComponent<Grid> ();
				if (grid != null) {
					int x;
					int y;

					Gridlike.GridUtility.GetClosestNonEmptyTile (grid, transform.position, out x, out y);
					if (grid != null) {
						GSGrid gsGrid = grid.GetComponent<GSGrid> ();

						gsGrid.Damage (character, x, y, damage, transform.position);
					}
				}
			} else {
				GridUtility.ExplodeInAllGrid (character, transform.position, radius, damage);
			}

			StartCoroutine (DestroyAnimation ());
		}

		IEnumerator DestroyAnimation() {

			yield return null;
			Destroy (gameObject);
		}

		IEnumerator DestroyAfterTime(int time) {
			yield return new WaitForSeconds (time);

			Destroy (gameObject);
		}
	}
}