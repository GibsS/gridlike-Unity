using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Gridlike;

public interface ICubeStorage {

	event Action<int> onAddCube;
	event Action<int> onRemoveCube;

	int GetCubeCount ();
	bool ConsumeCubes (int count);
	void AddCubes (int count);
}

public class GSCharacter : MonoBehaviour, ICubeStorage {

	public event Action<int> onAddCube;
	public event Action<int> onRemoveCube;

	public List<int> availableGridItems;

	public int cubeCount;

	public Bow bow;
	public Pickaxe pickaxe;
	public Placer placer;

	public Tool currentTool;

	public bool enableCameraZoomOut;

	PlatformerMotor2D motor;
	MovingPlatformMotor2D platform;

	void Start () {
		motor = GetComponent<PlatformerMotor2D> ();

		availableGridItems = new List<int> ();

		GSSingleton.instance.RegisterCharacter (this);

		if (bow != null) bow._Inject (this);
		if (pickaxe != null) pickaxe._Inject (this);
		if (placer != null) placer._Inject (this);
	}

	void Update() {
		if (currentTool != null) {
			if (Input.GetMouseButtonDown (0) && !IsPointerOverUIObject ()) {
				currentTool.OnMouseDown (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			}

			if (Input.GetMouseButton (0) && !IsPointerOverUIObject ()) {
				currentTool.OnMouse (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			}

			if (Input.GetMouseButtonUp (0) && !IsPointerOverUIObject ()) {
				currentTool.OnMouseUp (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			}

			if ((Input.GetMouseButtonDown (0) || Input.GetMouseButton (0) || Input.GetMouseButtonUp (0)) && !IsPointerOverUIObject ()) {
				currentTool.OnMouseAny (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			}
		}

		MovingPlatformMotor2D newPlatform = motor.connectedPlatform;
		if (platform != null && newPlatform == null) {
			GSShip ship = platform.GetComponent<GSShip> ();

			if (ship != null) {
				if(enableCameraZoomOut) Camera.main.orthographicSize /= 4;

				ship.hasCharacter = false;
			}

			platform = null;
		} else if (platform == null && newPlatform != null) {
			GSShip ship = newPlatform.GetComponent<GSShip> ();

			if (ship != null) {
				if(enableCameraZoomOut) Camera.main.orthographicSize *= 4;

				ship.hasCharacter = true;
			}

			platform = newPlatform;
		}
	}
	bool IsPointerOverUIObject() {

		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	#region CUBES

	public int GetCubeCount() {
		return cubeCount;
	}
	public bool ConsumeCubes(int count) {
		if (cubeCount >= count) {
			int old = cubeCount;
			cubeCount -= count;

			if (onRemoveCube != null) onRemoveCube (old);

			return true;
		} else {
			return false;
		}
	}
	public void AddCubes(int count) {
		int old = cubeCount;
		cubeCount += count;

		if (onAddCube != null) onAddCube (old);
	}

	#endregion

	#region ITEMS

	public bool HasBow() {
		return bow != null && bow.enabled && bow.acquired;
	}
	public bool HasPickaxe() {
		return pickaxe != null && pickaxe.enabled && pickaxe.acquired;
	}
	public bool HasPlacer() {
		return placer != null && placer.enabled && placer.acquired;
	}

	public void AcquireBow() {
		if (bow == null) bow = gameObject.AddComponent<Bow> ();
		bow._Inject (this);

		bow.enabled = true;
		bow.acquired = true;
	}
	public void AcquirePickaxe() {
		if (pickaxe == null) pickaxe = gameObject.AddComponent<Pickaxe> ();
		pickaxe._Inject (this);

		pickaxe.enabled = true;
		pickaxe.acquired = true;
	}
	public void AcquirePlacer() {
		if (placer == null) placer = gameObject.AddComponent<Placer> ();
		placer._Inject (this);

		placer.enabled = true;
		placer.acquired = true;
	}

	public void SelectBow() {
		currentTool = bow;
	}
	public void SelectPickaxe() {
		currentTool = pickaxe;
	}
	public void SelectPlacer() {
		currentTool = placer;
	}

	#endregion
}

