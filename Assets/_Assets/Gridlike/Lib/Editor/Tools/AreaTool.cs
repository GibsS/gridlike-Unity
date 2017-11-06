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

		bool deletePrevious; // true means at the end of dragging the selection, it get copied to the locatin, false means the selection gets moved
		bool copyEmpty;

		AreaToolState toolState;

		public override bool UseWindow () {
			return true;
		}
		public override string Name() {
			return "select";
		}

		public override bool Window () {
			deletePrevious = EditorGUILayout.Toggle ("Remove previous tiles", deletePrevious);
			copyEmpty = EditorGUILayout.Toggle ("Copy empty tiles", copyEmpty);

			if (GUILayout.Button ("Cancel selection")) {
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

				if (IsMouseInSelection (x, y)) {
					mouseInSelectionX = x - selectMinX;
					mouseInSelectionY = y - selectMinY;

					toolState = AreaToolState.DRAG;
				} else {
					selectStartX = mouseX;
					selectStartY = mouseY;

					CalculateSelectionBound (selectStartX, selectStartY);

					toolState = AreaToolState.SELECTING;
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
				CopyCurrentTo (mouseX - mouseInSelectionX, mouseY - mouseInSelectionY, deletePrevious);

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
			Vector2 bottomLeft = grid.transform.TransformPoint (new Vector2 (selectMinX, selectMinY));

			DrawSquare (bottomLeft.x, bottomLeft.y, bottomLeft.x + (selectMaxX - selectMinX), bottomLeft.y + (selectMaxY - selectMinY), Color.magenta);
		}
		void DrawDrag() {
			Vector2 bottomLeft = grid.transform.TransformPoint (new Vector2 (mouseX - mouseInSelectionX, mouseY - mouseInSelectionY));

			DrawSquare (bottomLeft.x, bottomLeft.y, bottomLeft.x + (selectMaxX - selectMinX), bottomLeft.y + (selectMaxY - selectMinY), Color.blue);
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

		void CopyCurrentTo(int x, int y, bool clear) {
			Tile[,] tiles = new Tile[selectMaxX - selectMinX, selectMaxY - selectMinY];

			for (int i = 0; i < selectMaxX - selectMinX; i++) {
				for (int j = 0; j < selectMaxY - selectMinY; j++) {
					Tile tile = grid.Get (selectMinX + i, selectMinY + j);

					if (tile != null && (copyEmpty || tile.id != 0)) {
						TileInfo info = grid.atlas [tile.id];

						if (info.tileGO == null || tile.tileGOCenter) {
							tiles[i, j] = new Tile {
								id = tile.id,
								subId = tile.subId,
								state1 = tile.state1,
								state2 = tile.state2,
								state3 = tile.state3,

								dictionary = tile.dictionary == null ? null : tile.dictionary.Clone()
							};
						}
					}

					if (clear) {
						grid.Clear (selectMinX + i, selectMinY + j);
					}
				}
			}

			for (int i = 0; i < tiles.GetLength (0); i++) {
				for (int j = 0; j < tiles.GetLength (1); j++) {
					if (tiles [i, j] != null) {
						grid.Set (x + i, y + j, tiles [i, j].id, tiles [i, j].subId, tiles [i, j].state1, tiles [i, j].state2, tiles [i, j].state3);
						grid.Get (x + i, y + j).dictionary = tiles [i, j].dictionary;
					}
				}
			}
		}
	}
}