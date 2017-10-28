using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshQuadTileMap : MonoBehaviour {

	public int width;
	public int height;

	public Texture2D texture;

	Mesh mesh;
	Vector3[] vertices;

	bool toggle;

	bool[,] clear;

	void Start () {
		clear = new bool[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (Random.Range (0f, 1f) > 0.8f) {
					clear [i, j] = true;
				}
			}
		}

		Generate ();

		texture = new Texture2D (2, 2);
		texture.filterMode = FilterMode.Point;
		texture.SetPixel (0, 0, Color.clear);
		texture.SetPixel (1, 0, Color.white);
		texture.SetPixel (0, 1, Color.red);
		texture.SetPixel (1, 1, Color.green);
		texture.Apply ();

		Material material = new Material (Shader.Find ("Sprites/Default"));
		material.mainTexture = texture;

		GetComponent<MeshRenderer> ().material = material;
	}
	void OnDrawGizmos () {
		if (vertices == null)
			return;

		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}

	void Update() {

		if (Input.GetKeyDown (KeyCode.A)) {
			StartCoroutine (SetTileAnimation1 ());
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			SetTileAnimation2 ();
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Generate ();
		}
	}

	IEnumerator SetTileAnimation1() {
		bool t = toggle = !toggle;
		var uv = GetComponent<MeshFilter>().sharedMesh.uv;

		for (int i = 1; i < width - 1; i++) {
			for (int j = 1; j < height - 1; j++) {
				SetTile (i, j, t);
				yield return null;
			}
		}

		GetComponent<MeshFilter>().sharedMesh.uv = uv;
	}

	void SetTileAnimation2() {
		var uv = GetComponent<MeshFilter>().sharedMesh.uv;

		for (int i = 1; i < width - 1; i++) {
			for (int j = 1; j < height - 1; j++) {
				OptimizedSetTile (uv, i, j);
			}
		}
		toggle = !toggle;

		GetComponent<MeshFilter>().sharedMesh.uv = uv;
	}

	void Generate() {

		var tileSize = 1;
		var tilesX = width;
		var tilesY = height;
		var quads = tilesX * tilesY; // one quad per tile

		var vertices = new Vector3[quads * 4];
		var triangles = new int[quads * 6];
		var normals = new Vector3[vertices.Length];
		var uv = new Vector2[vertices.Length];

		for (int y = 0; y < tilesY; y++)
		{
			for (int x = 0; x < tilesX; x++)
			{
				var i = (y * tilesX) + x; // quad index
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

		var mesh = new Mesh
		{
			vertices = vertices,
			triangles = triangles,
			normals = normals,
			uv = uv,
			name = "TileMapMesh"
		};

		GetComponent<MeshFilter>().mesh = mesh = mesh;
		mesh.name = "Procedural Grid";
	}

	public void SetTile(int x, int y, bool color)
	{
		int quadIndex = (y * width) + x;
		SetTile(quadIndex, color);
	}
	void SetTile(int quadIndex, bool color)
	{
		quadIndex *= 4;
		var uv = GetComponent<MeshFilter>().sharedMesh.uv;

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
		GetComponent<MeshFilter>().sharedMesh.uv = uv;
	}

	void OptimizedSetTile(Vector2[] uv, int x, int y) {

		int quadIndex = (y * width) + x;
		quadIndex *= 4;

		// assign four uv coordinates to change the texture of one tile (one quad, two triangels)
		if (!clear[x, y] && toggle) {
			uv [quadIndex] = new Vector2(0.6f, 0.6f);
			uv [quadIndex + 1] = new Vector2 (1f, 0.6f);
			uv [quadIndex + 2] = new Vector2 (0.6f, 1f);
			uv [quadIndex + 3] = new Vector2 (1f, 1f);
		} else {
			uv [quadIndex] = new Vector2(0, 0);
			uv [quadIndex + 1] = new Vector2 (0.4f, 0);
			uv [quadIndex + 2] = new Vector2 (0, 0.4f);
			uv [quadIndex + 3] = new Vector2 (0.4f, 0.4f);
		}
	}
}
