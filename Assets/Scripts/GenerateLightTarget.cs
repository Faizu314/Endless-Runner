using UnityEngine;

public class GenerateLightTarget : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;

    private void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[101];
        Vector2[] uv = new Vector2[101];
        int[] triangles = new int[100 * 3];
        int triangleIndex = 0;
        vertices[0] = Vector3.zero;
        uv[0] = Vector2.zero;
        for (int i = 1; i < 101; i++)
        {
            float theta = (i - 1) / 100f * 2 * Mathf.PI;
            vertices[i] = new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta));
            uv[i] = Vector2.zero;

            if (i != 1)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i;
                triangles[triangleIndex + 2] = i - 1;
                triangleIndex += 3;
            }
        }
        triangles[triangleIndex] = 0;
        triangles[triangleIndex + 1] = 1;
        triangles[triangleIndex + 2] = 100;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(250, 200, 0, 1));
        texture.Apply();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}

