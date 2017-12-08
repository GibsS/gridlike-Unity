using UnityEngine;
using System.Collections;

namespace Gridlike {

	/// <summary>
	/// A singleton for storing Gridlike's pool.
	/// </summary>
	public class GridlikePools : MonoBehaviour {

		ComponentPool<RegionMeshRenderer> _renderers;

		static GridlikePools _instance;

		/// <summary>
		/// Get region renderer pool.
		/// </summary>
		public static ComponentPool<RegionMeshRenderer> renderers {
			get { return _instance._renderers; }
		}

		public static GridlikePools instance {
			get {
				Initialize ();

				return _instance;
			}
		}

		/// <summary>
		/// Initialize the singleton. If necessary creates a GO to place the singleton on it.
		/// </summary>
		public static void Initialize() {
			if (_instance == null) {
				GameObject go = GameObject.Find ("Gridlike");
				if (go == null) go = new GameObject ("Gridlike");

				go.transform.position = Vector2.zero;
				_instance = go.AddComponent<GridlikePools> ();

				_instance._Initialize ();
			}
		}

		void _Initialize() {
			GameObject rendererContainer = new GameObject ("Mesh regions");
			rendererContainer.transform.SetParent (gameObject.transform);
			rendererContainer.transform.localPosition = Vector2.zero;
			_renderers = new ComponentPool<RegionMeshRenderer> (rendererContainer, 16, () => RegionMeshRenderer.Create(Grid.REGION_SIZE));
		}
	}
}