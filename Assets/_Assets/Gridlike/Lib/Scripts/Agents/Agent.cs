using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Used by Grid with agent based loading: those Grid will only load and present regions near agents.
	/// </summary>
	public class Agent : MonoBehaviour {

		/// <summary>
		/// Every agents in the current scene.
		/// </summary>
		public static List<Agent> agents { get; private set; }

		void Awake() {
			if (agents == null) agents = new List<Agent> ();

			agents.Add (this);
		}
	}
}