using UnityEngine;
using UnityEditor;
using System;

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
		Vector2 position = grid.transform.TransformPoint(new Vector2(x * grid.tileSize, y * grid.tileSize));

		Handles.color = color;

		Handles.DrawLine (new Vector2(position.x + 0.1f, position.y + 0.1f), new Vector2(position.x + 0.9f, position.y + 0.1f));
		Handles.DrawLine (new Vector2(position.x + 0.9f, position.y + 0.1f), new Vector2(position.x + 0.9f, position.y + 0.9f));
		Handles.DrawLine (new Vector2(position.x + 0.9f, position.y + 0.9f), new Vector2(position.x + 0.1f, position.y + 0.9f));
		Handles.DrawLine (new Vector2(position.x + 0.1f, position.y + 0.9f), new Vector2(position.x + 0.1f, position.y + 0.1f));

		if (text != null && text.Length > 0) {
			GUIStyle style = new GUIStyle ();
			style.normal.textColor = Color.white;

			for(int i = 0; i < text.Length; i++) {
				Handles.Label (new Vector2 (position.x + 1.1f, position.y + 1 - (i * 0.2f)), text[i], style);
			}
		}
	}

	public virtual int WindowHeight() { return 300; }
	public virtual bool UseWindow () { return false; }
	public virtual string Name() { return "change"; }

	public virtual bool Window () { return false; }

	public virtual bool Update() { return false; }

	public virtual bool OnMouseDown() { return false; }
	public virtual bool OnMouse() { return false; }
	public virtual bool OnMouseUp() { return false; }
}