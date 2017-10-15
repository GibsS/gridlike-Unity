using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

	public static List<Agent> agents { get; private set; }

	void Start() {
		if (agents == null) agents = new List<Agent> ();

		agents.Add (this);
	}
}
