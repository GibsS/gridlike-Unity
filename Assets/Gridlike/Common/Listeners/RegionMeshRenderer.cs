using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionMeshRenderer : MonoBehaviour {
	
	[HideInInspector] [SerializeField] int tilePerSide;

	[HideInInspector] [SerializeField] Texture2D texture;
	[HideInInspector] [SerializeField] int textureWidth;
	[HideInInspector] [SerializeField] int textureHeight;

	[HideInInspector] [SerializeField] Mesh mesh;
	[HideInInspector] [SerializeField] MeshFilter meshFilter;
	[HideInInspector] [SerializeField] MeshRenderer meshRenderer;

	Vector2[] uv;

	public static RegionMeshRenderer Create(int tilePerSide, Texture2D texture) {
		GameObject obj = new GameObject ("region mesh renderer");
		RegionMeshRenderer renderer = obj.AddComponent<RegionMeshRenderer> ();
		renderer.Initialize (tilePerSide, texture);
		return renderer;
	}

	public void Initialize(int tilePerCount, Texture2D texture) {
		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshRenderer = gameObject.AddComponent<MeshRenderer> ();

		this.tilePerSide = tilePerCount;
		this.texture = texture;

		textureWidth = texture.width;
		textureHeight = texture.height;

		Material material = new Material (Shader.Find ("Sprites/Default"));
		material.mainTexture = texture;

		meshRenderer.material = material;

		GenerateMesh ();
	}

	public void Destroy() {
		DestroyImmediate (gameObject);
	}

	void GenerateMesh() {

		var tileSize = 1;
		var quads = tilePerSide * tilePerSide;

		var vertices = new Vector3[quads * 4];
		var triangles = new int[quads * 6];
		var normals = new Vector3[vertices.Length];
		var uv = new Vector2[vertices.Length];

		for (int y = 0; y < tilePerSide; y++) {
			for (int x = 0; x < tilePerSide; x++) {
				var i = (y * tilePerSide) + x; // quad
				var qi = i * 4; // vertex
				var ti = i * 6;

				// 2--3
				// | /|
				// |/ |
				// 0--1
				var vx = x * tileSize;
				var vy = y * tileSize;
				vertices[qi] = new Vector3(vx, vy, 0);
				vertices[qi + 1] = new Vector3(vx + tileSize, vy, 0);
				vertices[qi + 2] = new Vector3(vx, vy + tileSize, 0);
				vertices[qi + 3] = new Vector3(vx + tileSize, vy + tileSize, 0);

				triangles[ti] = qi;
				triangles[ti + 1] = qi + 2;
				triangles[ti + 2] = qi + 3;

				triangles[ti + 3] = qi;
				triangles[ti + 4] = qi + 3;
				triangles[ti + 5] = qi + 1;
			}
		}

		for (int i = 0; i < vertices.Length; i++) {
			normals[i] = Vector3.forward;
			uv[i] = new Vector2(1, 1); // uv are set by assigning a tile
		}

		mesh = new Mesh {
			vertices = vertices,
			triangles = triangles,
			normals = normals,
			uv = uv,
			name = "Procedural Grid"
		};

		meshFilter.mesh = mesh;
	}

	public void PrepareUV() {
		uv = meshFilter.sharedMesh.uv;
	}

	public void ApplyUV() {
		meshFilter.sharedMesh.uv = uv;
	}

	public void SetTile(int x, int y, Sprite sprite) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		uv [quadIndex] 	   = new Vector2 (rect.xMin / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 1] = new Vector2 (rect.xMax / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 2] = new Vector2 (rect.xMin / textureWidth, rect.yMax / textureHeight);
		uv [quadIndex + 3] = new Vector2 (rect.xMax / textureWidth, rect.yMax / textureHeight);
	}
}
