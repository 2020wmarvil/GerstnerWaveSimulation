using UnityEngine;

// TODO: water material
// TODO: move to compute
// TODO: water physics (floating, pushing objects, etc.)
// TODO: play with gravity and other parts of our oceanographic spectum data to create an alien planet-esque system as opposed to an earth based system
// TODO: artisticly directed waves (big!)
// TODO: cresting waves
// TODO: tidal system
// TODO: splashes

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class OceanSurface : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] Vector2Int Size = new Vector2Int(6, 6);
    [SerializeField]float VertexSpacing = 1f;

    [Header("Waves")]
    [SerializeField] Vector2 windDirection = Vector2.one;
    [SerializeField] float steepness = 1f;
    [SerializeField] float wavelength = 1f;
    [SerializeField] float speed = 1f;

	MeshFilter filter;
	Mesh mesh;

    float PingPong;

    float G = 9.81f;
    float WindSpeed = 1f;

	void Awake() {
		filter = GetComponent<MeshFilter>();
		mesh = new Mesh();
	}

    void Start() {
        // initial spectrum h0(k) and h0(-k)
    }

	void Update() {
        // Fourier components h(k, t) 

        PingPong = 0f;

        float N = 1f;
        for (int I = 0; I < Mathf.Log(N, 2); ++I)
        {
            // horizontal butterfly for stage I
            PingPong = (PingPong + 1) % 2;
        }

        for (int I = 0; I < Mathf.Log(N, 2); ++I)
        {
            // vertical butterfly for stage I
            PingPong = (PingPong + 1) % 2;
        }

        // inversion and permutation

		GeneratePlaneMesh();
        LockToVertexGrid();
	}

    // TODO: this only needs to recalculate positions + normals
	void GeneratePlaneMesh() {
        float VertexDensity = 1f / VertexSpacing;

        Vector2Int NumTiles = new Vector2Int(
            Mathf.FloorToInt(Size.x * VertexDensity),
            Mathf.FloorToInt(Size.y * VertexDensity)
        );

        Vector2Int NumVerts = NumTiles + new Vector2Int(1, 1);

        int TotalNumTiles = NumTiles.x * NumTiles.y;
        int TotalNumVerts = NumVerts.x * NumVerts.y;

		Vector3[] Vertices = new Vector3[TotalNumVerts];
		Vector3[] Normals = new Vector3[TotalNumVerts];
		Vector2[] UVs = new Vector2[TotalNumVerts];
		int[] Triangles = new int[TotalNumTiles * 2 * 3];

		int TriIndex = 0; 
		for (int I = 0; I < TotalNumVerts; ++I) {
            int XIndex = I % NumVerts.x;
            int YIndex = I / NumVerts.x;

            float XPercent = (float)XIndex / NumVerts.x;
            float YPercent = (float)YIndex / NumVerts.y;

            Vector3 Position = new Vector3(
                Mathf.Lerp(-Size.x, Size.x, XPercent),
                0f,
                Mathf.Lerp(Size.y, -Size.y, YPercent)
            );

            Vector3 WavePosition = GerstnerWave.Gerstner(Position, windDirection, steepness, wavelength, speed, Time.time);

			Vertices[I] = Position + WavePosition;
			Normals[I] = Vector3.up;
			UVs[I] = new Vector2(XPercent, YPercent);

			if (XIndex < NumVerts.x - 1 && YIndex < NumVerts.y - 1) {
				Triangles[TriIndex++] = I;
				Triangles[TriIndex++] = I + 1;
				Triangles[TriIndex++] = I + 1 + NumVerts.x;

				Triangles[TriIndex++] = I;
				Triangles[TriIndex++] = I + 1 + NumVerts.x;
				Triangles[TriIndex++] = I + NumVerts.x;
			}
		}

		mesh.Clear();
		mesh.vertices = Vertices;
		mesh.normals = Normals;
		mesh.uv = UVs;
		mesh.triangles = Triangles;

		filter.mesh = mesh;
	}

    void LockToVertexGrid() {
        Vector3 ParentOffset = transform.parent.position;
        ParentOffset.x %= VertexSpacing;
        ParentOffset.y = 0f;
        ParentOffset.z %= VertexSpacing;
        transform.position = transform.parent.position - ParentOffset;
    }

    float AngularVelocity(float WaveNumber)
    {
        return Mathf.Sqrt(G * WaveNumber); // sqrt(g*k*tanh(k*h)), h = depth (we aassume infinite depth)
    }

    float Philip(Vector2 WaveVector) 
    {
        float L = WindSpeed * WindSpeed / G;

        float A = 1f;

        float k = WaveVector.magnitude;
        float kL = k * L;
        // im literally making shit up as i go
        float Result = A * Mathf.Exp(-1f / (kL * kL)) / (k*k*k*k) * Vector2.Dot(WaveVector, new Vector2(AngularVelocity(WaveVector.x), AngularVelocity(WaveVector.y)));

        return Result;
    }
}
