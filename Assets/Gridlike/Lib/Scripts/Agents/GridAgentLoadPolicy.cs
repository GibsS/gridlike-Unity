using UnityEngine;
using System.Collections;

// only use the first agent added 
public class GridAgentLoadPolicy {

	Grid grid;

	int oldX = 1000000000;
	int oldY = 1000000000;

	public GridAgentLoadPolicy(Grid grid) {
		this.grid = grid;
	}
	public void Update() {
		Agent agent = Agent.agents != null && Agent.agents.Count > 0 ? Agent.agents [0] : null;

		if (agent != null) {
			int X = Mathf.FloorToInt(agent.transform.position.x / Grid.REGION_SIZE);
			int Y = Mathf.FloorToInt(agent.transform.position.y / Grid.REGION_SIZE);

			if (oldX != X || oldY != Y) {
				UpdateFromPosition (X, Y);
			}
		}
	}
	void UpdateFromPosition(int X, int Y) {
		//Debug.LogWarning ("Update new position X=" + X + " Y=" + Y);
		this.oldX = X;
		this.oldY = Y;

		for (int i = -4; i <= 4; i++) {
			for (int j = -4; j <= 4; j++) {
				int dist = Mathf.Max (Mathf.Abs(i), Mathf.Abs(j));

				switch (dist) {
					case 0:
					case 1: {
						grid.PresentRegion (X + i, Y + j);
						break;
					}
					case 2: {
						grid.LoadRegion (X + i, Y + j);
						break;
					}
					case 3: {
						grid.HideRegion (X + i, Y + j);
						break;
					}
					case 4: {
						grid.SaveRegion (X + i, Y + j, true);
						break;
					}
				}
			}
		}
	}
}