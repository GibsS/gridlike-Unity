using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridship {

	public class ShipBarrier : MonoBehaviour {

		public float width;
		public float height;

		void Update() {
			GSShip ship = GSSingleton.instance.ship;

			if (ship != null) {
				Rigidbody2D body = ship.GetComponent<Rigidbody2D> ();
				Vector2 position = ship.transform.position;
				Vector2 barrierCenter = transform.position;

				float halfWidth = width / 2;
				float halfHeight = height / 2;

				if (position.x < barrierCenter.x - halfWidth && body.velocity.x < 0) {
					body.velocity = new Vector2 (0, body.velocity.y);
					// body.AddForce (FORCE * Vector2.right * Mathf.Exp(barrierCenter.x - halfWidth - position.x));
				}

				if (position.y < barrierCenter.y - halfHeight && body.velocity.y < 0) {
					body.velocity = new Vector2 (body.velocity.x, 0);
					//body.AddForce (FORCE * Vector2.up * Mathf.Exp(barrierCenter.y - halfHeight - position.y));
				}

				if (position.x > barrierCenter.x + halfWidth && body.velocity.x > 0) {
					body.velocity = new Vector2 (0, body.velocity.y);
					//body.AddForce (FORCE * Vector2.left * Mathf.Exp(position.x - (barrierCenter.x + halfWidth)));
				}

				if (position.y > barrierCenter.y + halfHeight && body.velocity.y > 0) {
					body.velocity = new Vector2 (body.velocity.x, 0);
					//body.AddForce (FORCE * Vector2.down * Mathf.Exp(position.y - (barrierCenter.y + halfHeight)));
				}
			}
		}

		void OnDrawGizmos() {
			Vector2 bottomLeft = transform.position + new Vector3 (-width / 2, -height / 2);
			Vector2 topLeft = transform.position + new Vector3 (-width / 2, height / 2);
			Vector2 bottomRight = transform.position + new Vector3 (width / 2, -height / 2);
			Vector2 topRight = transform.position + new Vector3 (width / 2, height / 2);

			Gizmos.DrawLine (bottomLeft, topLeft);
			Gizmos.DrawLine (topLeft, topRight);
			Gizmos.DrawLine (topRight, bottomRight);
			Gizmos.DrawLine (bottomRight, bottomLeft);
		}
	}
}
