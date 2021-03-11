using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public void DrawTestTerrain()
    {
        GameObject terrainObject = new GameObject("Terrain");
        terrainObject.layer = LayerMask.NameToLayer("Ground");
        MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = terrainObject.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-500, 0, 500),
            new Vector3(500, 0, 500),
            new Vector3(500, 0, -50),
            new Vector3(-500, 0, -50),
        };
        meshFilter.mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        meshFilter.mesh.RecalculateNormals();

        meshRenderer.material = MaterialHandler.Instance.DefaultMaterial;
        meshRenderer.material.color = MaterialHandler.Instance.TerrainColor;

        terrainObject.AddComponent<MeshCollider>();
    }
}
