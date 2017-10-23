using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class GridTool {

	public Grid _grid;

	public Grid grid { get { return _grid; } }

	public int mouseX {
		get {
			int mouseX, mouseY;

			grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out mouseX, out mouseY);

			return mouseX;
		}
	}
	public int mouseY {
		get {
			int mouseX, mouseY;

			grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out mouseX, out mouseY);

			return mouseY;
		}
	}

	public virtual bool UseWindow () { return false; }
	public virtual string Name() { return "change"; }

	public virtual void Window () { }

	public virtual void OnMouseDown() { }
	public virtual void OnMouse() { }
	public virtual void OnMouseUp() { }
}