using UnityEngine;
using UnityEditor;
using System;

namespace Gridlike {
	
	[Serializable]
	public class AreaTool : GridTool {

		enum AreaToolState {
			NONE, SELECTION, SELECTING, DRAG
		}

		int selectStartX;
		int selectStartY;

		int selectMinX;
		int selectMaxX;
		int selectMinY;
		int selectMaxY;

		int mouseInSelectionX;
		int mouseInSelectionY;

		bool copy; // true means at the end of dragging the selection, it get copied to the locatin, false means the selection gets moved
		bool copyEmpty;

		AreaToolState toolState;


		public override bool UseWindow () {
			return true;
		}
		public override string Name() {
			return "select";
		}

		public override bool Window () {
			copy = EditorGUILayout.Toggle ("copy", copy);
			copy = !EditorGUILayout.Toggle ("drag", !copy);
			copyEmpty = EditorGUILayout.Toggle ("copy empty tiles", copyEmpty);

			if (GUILayout.Button ("cancel selection")) {
				toolState = AreaToolState.NONE;
			}

			return false;
		}

		public override bool Update() {
			switch (toolState) {
			case AreaToolState.SELECTION:
				DrawSelection ();
				break;
			case AreaToolState.NONE:
				break;
			case AreaToolState.DRAG:
				DrawSelection ();
				DrawDrag ();
				break;
			case AreaToolState.SELECTING:
				DrawSelection ();
				break;
			}

			return false;
		}

		public override bool OnMouseDown() { 
			switch (toolState) {
			case AreaToolState.SELECTION:
				int x = mouseX;
				int y = mouseY;
				if (copy) {
					CopyCurrentTo (x, y);

					return true;
				} else {
					if (IsMouseInSelection (x, y)) {
						mouseInSelectionX = x - selectMinX;
						mouseInSelectionY = y - selectMinY;

						toolState = AreaToolState.DRAG;
					}
				}
				break;
			case AreaToolState.NONE:
				selectStartX = mouseX;
				selectStartY = mouseY;

				CalculateSelectionBound (selectStartX, selectStartY);

				toolState = AreaToolState.SELECTING;
				break;
			case AreaToolState.DRAG:
			case AreaToolState.SELECTING:
				break;
			}
			return false;
		}
		public override bool OnMouse() { 
			switch (toolState) {
			case AreaToolState.SELECTING:
				CalculateSelectionBound (mouseX, mouseY);
				break;
			case AreaToolState.DRAG:
			case AreaToolState.SELECTION:
			case AreaToolState.NONE:
				break;
			}
			return false;
		}
		public override bool OnMouseUp() { 
			switch (toolState) {
			case AreaToolState.DRAG:
				CopyCurrentTo (mouseX - mouseInSelectionX, mouseY - mouseInSelectionY, true);

				toolState = AreaToolState.NONE;
				return true;
			case AreaToolState.SELECTING:
				CalculateSelectionBound (mouseX, mouseY);

				toolState = AreaToolState.SELECTION;
				break;
			case AreaToolState.SELECTION:
			case AreaToolState.NONE:
				break;
			}
			return false;
		}

		void DrawSelection() {
			for (int i = selectMinX; i <= selectMaxX; i++) {
				for (int j = selectMinY; j <= selectMaxY; j++) {
					DrawTileInformation (i, j, Color.magenta, null);
				}
			}
		}
		void DrawDrag() {
			int x = mouseX;
			int y = mouseY;

			for (int i = 0; i <= selectMaxX - selectMinX; i++) {
				for (int j = 0; j <= selectMaxY - selectMinY; j++) {
					DrawTileInformation (x - mouseInSelectionX + i, y - mouseInSelectionY + j, Color.blue, null);
				}
			}
		}

		bool IsMouseInSelection(int x, int y) {
			return x >= selectMinX && x <= selectMaxX && y >= selectMinY && y <= selectMaxY;
		}

		void CalculateSelectionBound (int endX, int endY) {
			selectMinX = Mathf.Min (selectStartX, endX);
			selectMaxX = Mathf.Max (selectStartX, endX);
			selectMinY = Mathf.Min (selectStartY, endY);
			selectMaxY = Mathf.Max (selectStartY, endY);
		}

		void CopyCurrentTo(int x, int y, bool clear = false) {
			for (int i = 0; i <= selectMaxX - selectMinX; i++) {
				for (int j = 0; j <= selectMaxY - selectMinY; j++) {
					Tile tile = grid.Get (selectMinX + i, selectMinY + j);

					if (tile != null && (copyEmpty || tile.id != 0)) {
						TileInfo info = grid.atlas [tile.id];

						if (info.tileGO != null) {
							if (tile.tileGOCenter) {
								grid.Set (x + i, y + j, tile.id, tile.subId, tile.state1, tile.state2, tile.state3);
								if (tile.dictionary != null) {
									grid.Get (x + i, y + j).dictionary = tile.dictionary.Clone ();
								}
							}
						} else {
							grid.Set (x + i, y + j, tile.id, tile.subId, tile.state1, tile.state2, tile.state3);
						}
					}

					if (clear) {
						grid.Clear (selectMinX + i, selectMinY + j);
					}
				}
			}
		}
	}
}