using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BezierSurface_V2 : MonoBehaviour
{
    private const int gridSize = 4;

    [Header("Debug")]
    public bool drawHandles = false;
    public bool drawVertices = false;

    [Header("Settings")]
    public Mesh mesh;
    public Vector3[] controlVertices;
    [SerializeField] private Vector3 positionVertex;
    [SerializeField] public Vector3[] vertices;
    [SerializeField] private int[] triangles;

    public float speed = 5f;

    [Range(4, 16)]
    public int uResolution;

    [Range(4, 16)]
    public int vResolution;

    [Header("UVs")]
    [Range(0, 1)]
    public float u = 0;

    [Range(0, 1)]
    public float v = 0;

    void Start()
    {
        controlVertices = new Vector3[gridSize * gridSize];
        for (int i = 0, y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++, i++)
            {
                controlVertices[i] = new Vector3(x, 0f, y) + transform.position;
            }
        }

        mesh = new Mesh();
        RecalculateVertices();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void RecalculateVertices()
    {
        positionVertex = EvaluateBezierSurface(controlVertices, u, v);
        vertices = CalculateVertices(controlVertices, uResolution, vResolution);
        triangles = CalculateTriangles();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void Update()
    {
        RecalculateVertices();
    }

    private Vector3 EvaluateBezierSurface(Vector3[] p, float u, float v)
    {
        Vector3[] pu = new Vector3[4];

        for (int i = 0; i < 4; ++i)
        {
            Vector3[] curveP = new Vector3[4];
            curveP[0] = p[i * 4];
            curveP[1] = p[i * 4 + 1];
            curveP[2] = p[i * 4 + 2];
            curveP[3] = p[i * 4 + 3];
            pu[i] = EvaluateBezierCurve(curveP, u);
        }

        return EvaluateBezierCurve(pu, v);
    }

    private Vector3 EvaluateBezierCurve(Vector3[] p, float t)
    {
        float b0 = (1 - t) * (1 - t) * (1 - t);
        float b1 = 3 * t * (1 - t) * (1 - t);
        float b2 = 3 * t * t * (1 - t);
        float b3 = t * t * t;
        return p[0] * b0 + p[1] * b1 + p[2] * b2 + p[3] * b3;
    }

    private Vector3[] CalculateVertices(Vector3[] cV, int resU, int resV)
    {
        Vector3[] tmpVerts = new Vector3[(resU + 1) * (resV + 1)];

        int uDivs = resU - 1;
        int vDivs = resV - 1;

        for (int u = 0, i = 0; u <= uDivs; u++)
        {
            for(int v = 0; v <= vDivs; v++, i++)
            {
                tmpVerts[i] = EvaluateBezierSurface(cV, v / (float)vDivs, u / (float)uDivs);
            }
        }

        return tmpVerts;
    }

    private int[] CalculateTriangles()
    {
        int rows = uResolution - 1;
        int columns = vResolution - 1;
        int[] tmpTris = new int[rows * columns * 6];
        int index = 0;

        for (int i = 0; i < (vResolution * uResolution) - vResolution; ++i)
        {

            if ((i + 1) % vResolution == 0)
            {
                continue;
            }

            tmpTris[index + 0] = i;
            tmpTris[index + 1] = i + vResolution;
            tmpTris[index + 2] = i + vResolution + 1;

            tmpTris[index + 3] = i;
            tmpTris[index + 4] = i + vResolution + 1;
            tmpTris[index + 5] = i + 1;

            index += 6;
        }

        return tmpTris;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(positionVertex, 0.1f);

        if (!drawVertices)
        {
            return;
        }

        Gizmos.color = Color.black;
        foreach (Vector3 v in vertices)
        {
            Gizmos.DrawWireSphere(v, 0.1f);
        }
    }
}
