using UnityEngine;
using Unity.Mathematics;
using MathNet.Numerics;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour {

	public int xSize, ySize;

	private Mesh mesh;
	private Vector3[] vertices;

    const float dx = math.PI / 10f;
    const float dy = math.PI / 10f;
    private Mesh[] meshes;

    private Complex32 z1, z2;

    private void CalabiYau(float n, float a)
    {
        //Coordinate();

    }

    private Vector3 Coordinate(float x, float y, float n, float k1, float k2, float a)
    {
        z1 = Complex32.Multiply(
            Complex32.Exp(new Complex32(0, 2f * math.PI * k1 / n)),
            Complex32.Pow(Complex32.Sin(new Complex32(x, y)), 2 / n)
        );
        z2 = Complex32.Multiply(
            Complex32.Exp(new Complex32(0, 2f * math.PI * k2 / n)),
            Complex32.Pow(Complex32.Sin(new Complex32(x, y)), 2 / n)
        );
        return new Vector3(z1.Real, z2.Real, z1.Imaginary*math.cos(a) + z2.Imaginary*math.sin(a));
    }

    private void Awake () {
		Generate();
	}

	private void Generate () {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices[i] = new Vector3(x, y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				tangents[i] = tangent;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;

		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
        mesh.RecalculateTangents();
	}

}