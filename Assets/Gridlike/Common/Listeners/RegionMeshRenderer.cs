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

		int tileSize = 1;
		int quads = tilePerSide * tilePerSide;

		Vector3[] vertices = new Vector3[quads * 4];
		int[] triangles = new int[quads * 6];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector3[] normals = new Vector3[vertices.Length];

		// QUAD MESH STRUCTURE:
		// 2-3
		// |/|
		// 0-1
		for (int y = 0; y < tilePerSide; y++) {
			for (int x = 0; x < tilePerSide; x++) {
				int i = (y * tilePerSide) + x; // quad
				int qi = i * 4; // vertex
				int ti = i * 6;
				int vx = x * tileSize;
				int vy = y * tileSize;
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
			uv[i] = new Vector2(1, 1);
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
		// TODO if sprite is null, render the empty sprite
		if (sprite != null) {
			int quadIndex = ((y * tilePerSide) + x) * 4;

			Rect rect = sprite.textureRect;

			uv [quadIndex] = new Vector2 (rect.xMin / textureWidth, rect.yMin / textureHeight);
			uv [quadIndex + 1] = new Vector2 (rect.xMax / textureWidth, rect.yMin / textureHeight);
			uv [quadIndex + 2] = new Vector2 (rect.xMin / textureWidth, rect.yMax / textureHeight);
			uv [quadIndex + 3] = new Vector2 (rect.xMax / textureWidth, rect.yMax / textureHeight);
		}
	}
	public void SetPartialVerticalTile(int x, int y, Sprite sprite, int yTileOffset, int tilePixelSize) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		int minY = (int) (rect.yMin + yTileOffset * tilePixelSize);
		int maxY = (int) (rect.yMin + (yTileOffset + 1) * tilePixelSize);

		uv [quadIndex] = new Vector2 (rect.xMin / textureWidth, minY / textureHeight);
		uv [quadIndex + 1] = new Vector2 (rect.xMax / textureWidth, minY / textureHeight);
		uv [quadIndex + 2] = new Vector2 (rect.xMin / textureWidth, maxY / textureHeight);
		uv [quadIndex + 3] = new Vector2 (rect.xMax / textureWidth, maxY / textureHeight);
	}
	public void SetPartialHorizontalTile(int x, int y, Sprite sprite, int xTileOffset, int tilePixelSize) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		int minX = (int) (rect.xMin + xTileOffset * tilePixelSize);
		int maxX = (int) (rect.xMin + (xTileOffset + 1) * tilePixelSize);

		uv [quadIndex] = new Vector2 (minX, rect.yMin / textureHeight);
		uv [quadIndex + 1] = new Vector2 (maxX, rect.yMin / textureHeight);
		uv [quadIndex + 2] = new Vector2 (minX, rect.yMax / textureHeight);
		uv [quadIndex + 3] = new Vector2 (maxX, rect.yMax / textureHeight);
	}
}
