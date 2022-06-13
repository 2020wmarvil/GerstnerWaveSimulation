using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour {
	[SerializeField] int resolution = 16;

	MeshFilter filter;
	Mesh mesh;

	void Awake() {
		filter = GetComponent<MeshFilter>();
		mesh = new Mesh();
	}

	void Update() {
		GeneratePlaneMesh();
	}

	void GeneratePlaneMesh() {
		int NumVerts = (resolution+1) * (resolution+1);
		Vector3[] vertices = new Vector3[NumVerts];
		Vector3[] normals = new Vector3[NumVerts];
		Vector2[] uvs = new Vector2[NumVerts];
		int[] indices = new int[resolution*resolution*2*3];

		int vertexCount = 0;
		int indexCount = 0; 
		for (int r=0; r<=resolution; r++) {
			for (int c=0; c<=resolution; c++) {
				float percentX = (float)r / resolution;
				float percentY = (float)c / resolution;

				vertices[vertexCount] = new Vector3(percentX - 0.5f, 0f, percentY - 0.5f);
				normals[vertexCount] = Vector3.up;
				uvs[vertexCount] = new Vector2(r / resolution, c / resolution);

				print(vertices[vertexCount]);

				if (r < resolution && c < resolution) {
					indices[indexCount++] = vertexCount;
					indices[indexCount++] = vertexCount + 1;
					indices[indexCount++] = vertexCount + 1 + resolution+1;

					indices[indexCount++] = vertexCount;
					indices[indexCount++] = vertexCount + 1 + resolution+1;
					indices[indexCount++] = vertexCount + resolution+1;
				}

				vertexCount++;
			}
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = indices;

		filter.mesh = mesh;
	}
}
