using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionMeshRenderer : MonoBehaviour {
	
	[HideInInspector] [SerializeField] int tilePerSide;

	[HideInInspector] [SerializeField] Texture2D texture;

	[HideInInspector] [SerializeField] Mesh mesh;
	[HideInInspector] [SerializeField] MeshFilter meshFilter;
	[HideInInspector] [SerializeField] MeshRenderer meshRenderer;

	Vector3[] vertices;

	public void Initialize(int tilePerCount, Texture2D texture) {
		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshRenderer = gameObject.AddComponent<MeshRenderer> ();

		this.tilePerSide = tilePerCount;
		this.texture = texture;

		Material material = new Material (Shader.Find ("Sprites/Default"));
		material.mainTexture = texture;

		meshRenderer.material = material;

		GenerateMesh ();
	}

	void GenerateMesh() {

		var tileSize = 1;
		var quads = tilePerSide * tilePerSide; // one quad per tile

		vertices = new Vector3[quads * 4];
		var triangles = new int[quads * 6];
		var normals = new Vector3[vertices.Length];
		var uv = new Vector2[vertices.Length];

		for (int y = 0; y < tilePerSide; y++)
		{
			for (int x = 0; x < tilePerSide; x++)
			{
				var i = (y * tilePerSide) + x; // quad index
				var qi = i * 4; // vertex index
				var ti = i * 6;

				// vertices going clockwise
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

		for (int i = 0; i < vertices.Length; i++)
		{
			normals[i] = Vector3.forward;
			uv[i] = new Vector2(1, 1); // uv are set by assigning a tile
		}

		mesh = new Mesh
		{
			vertices = vertices,
			triangles = triangles,
			normals = normals,
			uv = uv,
			name = "Procedural Grid"
		};

		meshFilter.mesh = mesh;
	}

	bool toggle;
	public void TEST_SetTiles() {
		var uv = meshFilter.sharedMesh.uv;

		for (int i = 1; i < tilePerSide - 1; i++) {
			for (int j = 1; j < tilePerSide - 1; j++) {
				OptimizedSetTile (uv, i, j);
			}
		}
		toggle = !toggle;

		meshFilter.sharedMesh.uv = uv;
	}
	public void TEST2_SetTiles() {
		for (int i = 1; i < tilePerSide - 1; i++) {
			for (int j = 1; j < tilePerSide - 1; j++) {
				var uv = meshFilter.sharedMesh.uv;
				OptimizedSetTile (uv, i, j);
				meshFilter.sharedMesh.uv = uv;
			}
		}
	}

	public void SetTile(int x, int y, bool color)
	{
		int quadIndex = (y * tilePerSide) + x;
		SetTile(quadIndex, color);
	}
	void SetTile(int quadIndex, bool color)
	{
		quadIndex *= 4;
		var uv = meshFilter.sharedMesh.uv;

		// assign four uv coordinates to change the texture of one tile (one quad, two triangels)
		if (color) {
			uv [quadIndex] = new Vector2(0.5f, 0.5f);
			uv [quadIndex + 1] = new Vector2 (1f, 0.5f);
			uv [quadIndex + 2] = new Vector2 (0.5f, 1f);
			uv [quadIndex + 3] = new Vector2 (1f, 1f);
		} else {
			uv [quadIndex] = new Vector2(0, 0);
			uv [quadIndex + 1] = new Vector2 (0.5f, 0);
			uv [quadIndex + 2] = new Vector2 (0, 0.5f);
			uv [quadIndex + 3] = new Vector2 (0.5f, 0.5f);
		}
		meshFilter.sharedMesh.uv = uv;
	}

	void OptimizedSetTile(Vector2[] uv, int x, int y) {

		int quadIndex = (y * tilePerSide) + x;
		quadIndex *= 4;

		// assign four uv coordinates to change the texture of one tile (one quad, two triangels)
		float s = 16f/256f;
		if (toggle) {
			uv [quadIndex] = new Vector2 (s, s);
			uv [quadIndex + 1] = new Vector2 (2*s, s);
			uv [quadIndex + 2] = new Vector2 (s, 2*s);
			uv [quadIndex + 3] = new Vector2 (2*s, 2*s);
		} else {
			uv [quadIndex] = new Vector2 (0, 0);
			uv [quadIndex + 1] = new Vector2 (s, 0);
			uv [quadIndex + 2] = new Vector2 (0, s);
			uv [quadIndex + 3] = new Vector2 (s, s);
		}
	}
}
