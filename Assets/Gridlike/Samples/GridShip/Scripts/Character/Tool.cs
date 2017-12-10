using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridship {

	public class Tool : MonoBehaviour {

		protected GSCharacter character { get; private set; }

		public bool acquired;

		public void _Inject(GSCharacter character) {
			this.character = character;
		}

		public virtual void OnMouseDown(Vector2 position) {

		}
		public virtual void OnMouse(Vector2 position) {

		}
		public virtual void OnMouseUp(Vector2 position) {

		}
		public virtual void OnMouseAny(Vector2 position) {

		}
	}
}