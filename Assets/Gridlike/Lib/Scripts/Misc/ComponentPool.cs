using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gridlike {

	/// <summary>
	/// System to pool GOs.
	/// </summary>
	public class ComponentPool<X> where X : Component {

		List<X> components;

		Factory<X> factory;

		GameObject container;

		public ComponentPool(GameObject container, int startCount, Factory<X> factory) {
			this.container = container;

			components = new List<X>();
			this.factory = factory;

			for (int i = 0; i < startCount; i++) {
				X x = factory ();

				x.gameObject.SetActive (false);
				x.gameObject.transform.SetParent (container.transform);

				components.Add (x);
			}
		}

		/// <summary>
		/// Gets an instance of X. Creates it if none are left in the pool.
		/// </summary>
		public X Get() {
			if (components.Count > 0) {
				X x = components [0];
				components.RemoveAt (0);
				x.gameObject.SetActive (true);
				return x;
			} else {
				return factory ();
			}
		}
		/// <summary>
		/// Removes the GO x is attached by disabling it and adding it to the pool of available GOs.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		public void Free(X x) {
			x.gameObject.SetActive (false);
			x.gameObject.transform.SetParent (container.transform);
			components.Add (x);
		}

		/// <summary>
		/// Clears the pool.
		/// </summary>
		public void Clear() {
			foreach (X x in components) {
				if (x != null && x.gameObject != null) {
					UnityEngine.Object.Destroy (x.gameObject);
				}
			}
			components.Clear ();
		}
	}
}