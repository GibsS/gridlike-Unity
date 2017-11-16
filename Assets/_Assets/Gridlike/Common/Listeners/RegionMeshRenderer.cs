using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionMeshRenderer : MonoBehaviour {
	
	[HideInInspector, SerializeField] int tilePerSide;

	[HideInInspector, SerializeField] int textureWidth;
	[HideInInspector, SerializeField] int textureHeight;
	[HideInInspector, SerializeField] int tilePixelSize;

	[HideInInspector, SerializeField] Mesh mesh;
	[HideInInspector, SerializeField] MeshFilter meshFilter;
	[HideInInspector, SerializeField] MeshRenderer meshRenderer;

	Vector2[] uv;

	Vector2 bottomLeftEmpty;
	Vector2 bottomRightEmpty;
	Vector2 topLeftEmpty;
	Vector2 topRightEmpty;

	public static RegionMeshRenderer Create(int tilePerSide) {
		GameObject obj = new GameObject ("region mesh renderer");
		RegionMeshRenderer renderer = obj.AddComponent<RegionMeshRenderer> ();

		renderer.Setup (tilePerSide);

		return renderer;
	}
	void Setup(int tilePerSide) {
		this.tilePerSide = tilePerSide;
		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshRenderer = gameObject.AddComponent<MeshRenderer> ();

		GenerateMesh ();
	}

	public void Initialize(Material material, int tilePixelSize, Sprite emptySprite) {
		textureWidth = material.mainTexture.width;
		textureHeight = material.mainTexture.height;
		this.tilePixelSize = tilePixelSize;

		meshRenderer.material = material;

		Rect rect = emptySprite.textureRect;

		bottomLeftEmpty = new Vector2 (rect.xMin / textureWidth, rect.yMin / textureHeight);
		bottomRightEmpty = new Vector2 (rect.xMax / textureWidth, rect.yMin / textureHeight);
		topLeftEmpty = new Vector2 (rect.xMin / textureWidth, rect.yMax / textureHeight);
		topRightEmpty = new Vector2 (rect.xMax / textureWidth, rect.yMax / textureHeight);
	}

	public void Destroy() {
		if (Application.isPlaying)
			Destroy (gameObject);
		else
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

	public void Clear(int x, int y) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		uv [quadIndex] = bottomLeftEmpty;
		uv [quadIndex + 1] = bottomRightEmpty;
		uv [quadIndex + 2] = topLeftEmpty;
		uv [quadIndex + 3] = topRightEmpty;
	}

	public void SetTile(int x, int y, Sprite sprite) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		uv [quadIndex] = new Vector2 (rect.xMin / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 1] = new Vector2 (rect.xMax / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 2] = new Vector2 (rect.xMin / textureWidth, rect.yMax / textureHeight);
		uv [quadIndex + 3] = new Vector2 (rect.xMax / textureWidth, rect.yMax / textureHeight);
	}
	public void SetPartialVerticalTile(int x, int y, Sprite sprite, int yTileOffset) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		float minY = (rect.yMin + yTileOffset * tilePixelSize);
		float maxY = (rect.yMin + (yTileOffset + 1) * tilePixelSize);

		uv [quadIndex] = new Vector2 (rect.xMin / textureWidth, minY / textureHeight);
		uv [quadIndex + 1] = new Vector2 (rect.xMax / textureWidth, minY / textureHeight);
		uv [quadIndex + 2] = new Vector2 (rect.xMin / textureWidth, maxY / textureHeight);
		uv [quadIndex + 3] = new Vector2 (rect.xMax / textureWidth, maxY / textureHeight);
	}
	public void SetPartialHorizontalTile(int x, int y, Sprite sprite, int xTileOffset) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		float minX = (rect.xMin + xTileOffset * tilePixelSize);
		float maxX = (rect.xMin + (xTileOffset + 1) * tilePixelSize);

		//Debug.Log ("sprite=" + sprite + " minx=" + minX + " maxx=" + maxX + " miny=" + rect.yMin + " maxy=" + rect.yMax);

		uv [quadIndex] = new Vector2 (minX / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 1] = new Vector2 (maxX / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 2] = new Vector2 (minX / textureWidth, rect.yMax / textureHeight);
		uv [quadIndex + 3] = new Vector2 (maxX / textureWidth, rect.yMax / textureHeight);
	}
}
