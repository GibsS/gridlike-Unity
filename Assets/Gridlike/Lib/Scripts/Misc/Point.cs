using UnityEngine;
using System;

namespace Gridlike {

	/// <summary>
	/// Implementation of a immutable Point (pair of integer coordinates)
	/// </summary>
	[Serializable]
	public class Point : IEquatable<Point> {

		[SerializeField] public int x { get; private set; }
		[SerializeField] public int y { get; private set; }

		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override string ToString() {
			return "[x=" + x + "; y=" + y + "]";
		}
		public override bool Equals(object obj) {
			if (obj == null) return false;
			Point objAsPart = obj as Point;
			if (objAsPart == null) return false;
			else return Equals(objAsPart);
		}
		public override int GetHashCode() {
			return x^y;
		}
		public bool Equals(Point other) {
			if (other == null) return false;
			return x == other.x && y == other.y;
		}
		public static bool operator==(Point p1, Point p2) {
			if (System.Object.ReferenceEquals(p1, p2)) return true;

			// If one is null, but not both, return false.
			if (((object) p1 == null) || ((object) p2 == null)) return false;

			// Return true if the fields match:
			return p1.x == p2.x && p1.y == p2.y;
		}
		public static bool operator!=(Point p1, Point p2) {
			return !(p1 == p2);
		}
	}
}