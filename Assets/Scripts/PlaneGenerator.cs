using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    [SerializeField] private Vector2 flatZone;
    [SerializeField] private Vector2 flatZoneOrigin;

    int sizeXMesh;
    int sizeZMesh;

    float flatZoneMaxX = 0;
    float flatZoneMinX = 0;
    float flatZoneMaxZ = 0;
    float flatZoneMinZ = 0;

    private MeshFilter meshFilter;

    public void CreatePlane()
    {
        Mesh mesh = new Mesh();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        sizeXMesh = sizeX;
        sizeZMesh = sizeZ;

        Vector3[] vertices = new Vector3[(sizeXMesh + 1) * (sizeZMesh + 1)];
        Vector2[] uv = new Vector2[(sizeXMesh + 1) * (sizeZMesh + 1)];
        int[] triangles = new int[sizeXMesh * sizeZMesh * 6];

        for (int z = 0; z < sizeZMesh + 1; z++)
        {
            for (int x = 0; x < sizeXMesh + 1; x++)
            {
                ref Vector3 vertice = ref vertices[x + z * (sizeXMesh + 1)];
                vertice.x = x;
                vertice.y = 0;
                vertice.z = z;
                ref Vector2 _uv = ref uv[x + z * (sizeXMesh + 1)];
                _uv.x = (float)x / sizeXMesh;
                _uv.y = (float)z / sizeZMesh;
            }
        }

        int indexTriangle = 0;

        for (int z = 0; z < sizeZMesh; z++)
        {
            for (int x = 0; x < sizeXMesh; x++)
            {
                int bottomLeftIndex = x + z * (sizeXMesh + 1);
                int bottomRightIndex = x + 1 + z * (sizeXMesh + 1);
                int topLeftIndex = x + (z + 1) * (sizeXMesh + 1);
                int topRightIndex = x + 1 + (z + 1) * (sizeXMesh + 1);

                triangles[indexTriangle] = bottomLeftIndex;
                triangles[indexTriangle + 1] = topLeftIndex;
                triangles[indexTriangle + 2] = bottomRightIndex;

                triangles[indexTriangle + 3] = bottomRightIndex;
                triangles[indexTriangle + 4] = topLeftIndex;
                triangles[indexTriangle + 5] = topRightIndex;

                indexTriangle += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        meshFilter.mesh = mesh;

        //Dirty but works. We could calculate the bounds, but since we will always look at the road it's not necessary
        Bounds newBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
        mesh.bounds = newBounds;
    }

    private void OnValidate()
    {
        flatZoneMaxX = flatZoneOrigin.x + flatZone.x / 2;
        flatZoneMinX = flatZoneOrigin.x - flatZone.x / 2;
        flatZoneMaxZ = flatZoneOrigin.y + flatZone.y / 2;
        flatZoneMinZ = flatZoneOrigin.y - flatZone.y / 2;

        if (sizeX != sizeXMesh || sizeZ != sizeZMesh)
        {
            CreatePlane();
        }
    }

    bool IsInsideFlatZone(int x, int z)
    {
        return (x > flatZoneMaxX || x < flatZoneMinX || z > flatZoneMaxZ || z < flatZoneMinZ);
    }
}
