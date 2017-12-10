using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Component for generating a mesh representing a tile map. 
/// </summary>
public class RegionMeshRenderer : MonoBehaviour {

	/// <summary>
	/// The size in tiles of the mesh.
	/// </summary>
	[HideInInspector, SerializeField] int tilePerSide;

	/// <summary>
	/// The cached width size.
	/// </summary>
	[HideInInspector, SerializeField] int textureWidth;
	/// <summary>
	/// The cached height size.
	/// </summary>
	[HideInInspector, SerializeField] int textureHeight;
	/// <summary>
	/// The size of a tile in pixel.
	/// </summary>
	[HideInInspector, SerializeField] int tilePixelSize;

	/// <summary>
	/// The mesh.
	/// </summary>
	[HideInInspector, SerializeField] Mesh mesh;
	/// <summary>
	/// The mesh filter.
	/// </summary>
	[HideInInspector, SerializeField] MeshFilter meshFilter;
	/// <summary>
	/// The mesh renderer.
	/// </summary>
	[HideInInspector, SerializeField] MeshRenderer meshRenderer;

	Vector2[] uv;

	Vector2 bottomLeftEmpty;
	Vector2 bottomRightEmpty;
	Vector2 topLeftEmpty;
	Vector2 topRightEmpty;

	public static RegionMeshRenderer Create(int tilePerSide) {
		GameObject obj = new GameObject ("region mesh renderer");
		RegionMeshRenderer renderer = obj.AddComponent<RegionMeshRenderer> ();

		renderer.SetupMesh (tilePerSide);

		return renderer;
	}

	/// <summary>
	/// Creates the necessary components and generates the mesh.
	/// </summary>
	/// <param name="tilePerSide">Tile per side.</param>
	void SetupMesh(int tilePerSide) {
		this.tilePerSide = tilePerSide;
		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshRenderer = gameObject.AddComponent<MeshRenderer> ();

		GenerateMesh ();
	}

	/// <summary>
	/// Collects the rendering data.
	/// </summary>
	/// <param name="material">The material with the sprite sheet on it.</param>
	/// <param name="tilePixelSize">The pixel size of a tile in the sprite sheet</param>
	/// <param name="emptySprite">A reference to the empty sprite in the sprite sheet.</param>
	public void SetupRendering(Material material, int tilePixelSize, Sprite emptySprite) {
		textureWidth = material.mainTexture.width;
		textureHeight = material.mainTexture.height;
		this.tilePixelSize = tilePixelSize;

		meshRenderer.material = material;

		Rect rect = emptySprite.textureRect;

		bottomLeftEmpty = new Vector2 ((rect.xMin + 0.6f) / textureWidth, (rect.yMin + 0.6f) / textureHeight);
		bottomRightEmpty = new Vector2 ((rect.xMax - 0.6f) / textureWidth, (rect.yMin + 0.6f) / textureHeight);
		topLeftEmpty = new Vector2 ((rect.xMin + 0.6f) / textureWidth, (rect.yMax - 0.6f) / textureHeight);
		topRightEmpty = new Vector2 ((rect.xMax - 0.6f) / textureWidth, (rect.yMax - 0.6f) / textureHeight);
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

	/// <summary>
	/// To be called before setting tiles.
	/// </summary>
	public void PrepareUV() {
		uv = meshFilter.sharedMesh.uv;
	}

	/// <summary>
	/// To be called once defining the tiles is done.
	/// </summary>
	public void ApplyUV() {
		meshFilter.sharedMesh.uv = uv;
	}

	/// <summary>
	/// Clears the specified tile.
	/// </summary>
	/// <param name="x">The x tile coordinate.</param>
	/// <param name="y">The y tile coordinate.</param>
	public void Clear(int x, int y) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		uv [quadIndex] = bottomLeftEmpty;
		uv [quadIndex + 1] = bottomRightEmpty;
		uv [quadIndex + 2] = topLeftEmpty;
		uv [quadIndex + 3] = topRightEmpty;
	}

	/// <summary>
	/// Defines the sprite to render for the specified tile.
	/// </summary>
	/// <param name="x">The x tile coordinate.</param>
	/// <param name="y">The y tile coordinate.</param>
	/// <param name="sprite">The sprite, needs to be part of the spritesheet.</param>
	public void SetTile(int x, int y, Sprite sprite) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		uv [quadIndex] = new Vector2 ((rect.xMin + 0.6f) / textureWidth, (rect.yMin + 0.6f) / textureHeight);
		uv [quadIndex + 1] = new Vector2 ((rect.xMax - 0.6f) / textureWidth, (rect.yMin + 0.6f) / textureHeight);
		uv [quadIndex + 2] = new Vector2 ((rect.xMin + 0.6f) / textureWidth, (rect.yMax - 0.6f) / textureHeight);
		uv [quadIndex + 3] = new Vector2 ((rect.xMax - 0.6f) / textureWidth, (rect.yMax - 0.6f) / textureHeight);
	}
	public void SetPartialVerticalTile(int x, int y, Sprite sprite, int yTileOffset) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		float minY = (rect.yMin + yTileOffset * tilePixelSize) + 0.4f;
		float maxY = (rect.yMin + (yTileOffset + 1) * tilePixelSize) - 0.4f;

		uv [quadIndex] = new Vector2 ((rect.xMin + 0.6f) / textureWidth, minY / textureHeight);
		uv [quadIndex + 1] = new Vector2 ((rect.xMax - 0.6f) / textureWidth, minY / textureHeight);
		uv [quadIndex + 2] = new Vector2 ((rect.xMin + 0.6f) / textureWidth, maxY / textureHeight);
		uv [quadIndex + 3] = new Vector2 ((rect.xMax - 0.6f) / textureWidth, maxY / textureHeight);
	}
	public void SetPartialHorizontalTile(int x, int y, Sprite sprite, int xTileOffset) {
		int quadIndex = ((y * tilePerSide) + x) * 4;

		Rect rect = sprite.textureRect;

		float minX = (rect.xMin + xTileOffset * tilePixelSize) + 0.4f;
		float maxX = (rect.xMin + (xTileOffset + 1) * tilePixelSize) - 0.4f;

		uv [quadIndex] = new Vector2 (minX / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 1] = new Vector2 (maxX / textureWidth, rect.yMin / textureHeight);
		uv [quadIndex + 2] = new Vector2 (minX / textureWidth, rect.yMax / textureHeight);
		uv [quadIndex + 3] = new Vector2 (maxX / textureWidth, rect.yMax / textureHeight);
	}
}
