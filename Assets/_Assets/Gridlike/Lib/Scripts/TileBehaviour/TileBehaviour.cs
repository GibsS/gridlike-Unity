using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	public abstract class TileBehaviour : MonoBehaviour {

		public Grid _grid;
		public int _x;
		public int _y;

		Tile _tile;

		public int x { get { return _x; } }
		public int y { get { return _y; } }

		public Grid grid { get { return _grid; } }
		public Tile tile { 
			get { 
				if (_tile == null) _tile = _grid.Get (_x, _y);
				
				return _tile; 
			} 
		}

		public abstract bool[,] area { get; }
		public abstract int areaBottomLeftXOffset { get; }
		public abstract int areaBottomLeftYOffset { get; }

		public void Destroy() {
			grid.Clear (_x, _y);
		}

		public virtual void OnShow() { }
		public virtual void OnHide() { }
	}
}