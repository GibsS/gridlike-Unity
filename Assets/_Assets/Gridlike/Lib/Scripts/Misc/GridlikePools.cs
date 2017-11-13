using UnityEngine;
using System.Collections;

namespace Gridlike {

	public class GridlikePools : MonoBehaviour {

		ComponentPool<RegionMeshRenderer> _renderers;

		static GridlikePools _instance;

		public static ComponentPool<RegionMeshRenderer> renderers {
			get { return _instance._renderers; }
		}

		public static GridlikePools instance {
			get {
				Initialize ();

				return _instance;
			}
		}

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