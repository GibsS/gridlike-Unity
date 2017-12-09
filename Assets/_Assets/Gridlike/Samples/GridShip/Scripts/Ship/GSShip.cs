using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

namespace Gridship {

	public class GSShip : MonoBehaviour {

		Rigidbody2D rigidbody;
		MovingPlatformMotor2D platformer;
		Grid grid;

		public float maxMass;
		public float propulsionForce;

		[HideInInspector] public bool hasCharacter = true;

		[HideInInspector] public bool pushLeft;
		[HideInInspector] public bool pushRight;
		[HideInInspector] public bool pushUp;
		[HideInInspector] public bool pushDown;

		void Start() {
			rigidbody = GetComponent<Rigidbody2D> ();
			platformer = GetComponent<MovingPlatformMotor2D> ();
			grid = GetComponent<Grid> ();

			GSSingleton.instance.RegisterShip (this);
		}

		void Update() {
			InputHandling ();

			Physics ();
		}

		void InputHandling() {
			pushLeft = Input.GetKey (KeyCode.F) && !Input.GetKey (KeyCode.H);
			pushRight = Input.GetKey (KeyCode.H) && !Input.GetKey (KeyCode.F);

			pushUp = Input.GetKey (KeyCode.T) && !Input.GetKey (KeyCode.G);
			pushDown = Input.GetKey (KeyCode.G) && !Input.GetKey (KeyCode.T);
		}
		void Physics() {

			if (rigidbody.mass > maxMass) {
				rigidbody.AddForce (10 * (rigidbody.mass -  maxMass) * Vector2.down);
			}

			if (hasCharacter) {
				if (pushLeft)
					rigidbody.AddForce (propulsionForce * Vector2.left);
				if (pushRight)
					rigidbody.AddForce (propulsionForce * Vector2.right);
				if (pushUp)
					rigidbody.AddForce (propulsionForce * Vector2.up);
				if (pushDown)
					rigidbody.AddForce (propulsionForce * Vector2.down);
			}
		}
	}
}