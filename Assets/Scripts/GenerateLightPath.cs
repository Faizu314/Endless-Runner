using UnityEngine;

public class GenerateLightPath : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private AnimationCurve gradientMap;

    private void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, 0, -1);
        vertices[1] = new Vector3(0.5f, 0, -1);
        vertices[2] = new Vector3(-1, 0, 1);
        vertices[3] = new Vector3(1, 0, 1);
        mesh.vertices = vertices;

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 3;
        triangles[3] = 1;
        triangles[4] = 0;
        triangles[5] = 3;
        mesh.triangles = triangles;

        Texture2D texture = new Texture2D(1000, 1);
        for (int i = 0; i < 1000; i++)
            texture.SetPixel(i, 0, new Color(250, 200, 0, gradientMap.Evaluate((float)(i / 1000f))));
        texture.Apply();

        Vector2[] uv = new Vector2[4];
        uv[0] = Vector2.zero;
        uv[1] = Vector2.zero;
        uv[2] = Vector2.right;
        uv[3] = Vector2.right;
        mesh.uv = uv;

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
