namespace Gridlike {

	public enum TileShape {
		EMPTY, 
		FULL, 

		UP_ONEWAY, 
		DOWN_ONEWAY, 
		LEFT_ONEWAY,
		RIGHT_ONEWAY,

		UP_LEFT_TRIANGLE,
		UP_RIGHT_TRIANGLE,
		DOWN_LEFT_TRIANGLE,
		DOWN_RIGHT_TRIANGLE
	}

	public static class TileShapeHelper {

		public static bool IsTriangle(TileShape shape) {
			return (int)shape >= 6;
		}

		public static bool IsOneway(TileShape shape) {
			return (int)shape >= 2 && (int)shape < 6;
		}
	}
}