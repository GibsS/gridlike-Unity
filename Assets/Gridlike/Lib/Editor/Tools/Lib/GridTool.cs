using UnityEngine;
using UnityEditor;
using System;

namespace Gridlike {

	[Serializable]
	public abstract class GridTool {

		public Grid _grid;

		public Grid grid { get { return _grid; } }

		protected int mouseX {
			get {
				int mouseX, mouseY;

				grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out mouseX, out mouseY);

				return mouseX;
			}
		}
		protected int mouseY {
			get {
				int mouseX, mouseY;

				grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out mouseX, out mouseY);

				return mouseY;
			}
		}

		protected void DrawTileInformation(int x, int y, Color color, string[] text) {
			Vector2 position = grid.transform.TransformPoint(new Vector2(x, y));

			DrawSquare (position.x, position.y, position.x + 1, position.y + 1, color);

			if (text != null && text.Length > 0) {
				GUIStyle style = new GUIStyle ();
				style.normal.textColor = Color.white;

				for(int i = 0; i < text.Length; i++) {
					Handles.Label (new Vector2 (position.x + 1.1f, position.y + 1 - (i * 0.2f)), text[i], style);
				}
			}
		}
		protected void DrawSquare(float minx, float miny, float maxx, float maxy, Color color) {

			Handles.color = color;

			Handles.DrawLine (new Vector2(minx, miny), new Vector2(maxx, miny));
			Handles.DrawLine (new Vector2(maxx, miny), new Vector2(maxx, maxy));
			Handles.DrawLine (new Vector2(maxx, maxy), new Vector2(minx, maxy));
			Handles.DrawLine (new Vector2(minx, maxy), new Vector2(minx, miny));
		}

		public virtual void Serialize() { }

		public virtual int WindowHeight() { return 300; }
		public virtual bool UseWindow () { return false; }
		public virtual string Name() { return "change"; }

		public virtual bool Window () { return false; }

		public virtual bool Update() { return false; }

		public virtual bool OnMouseDown() { return false; }
		public virtual bool OnMouse() { return false; }
		public virtual bool OnMouseUp() { return false; }
	}
}