using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshGenerator
{
    public static GameObject CreateEmptyObject(string name, string layer, Transform parent = null)
    {
        GameObject newObject = new GameObject(name);
        newObject.layer = LayerMask.NameToLayer(layer);
        if(parent != null) newObject.transform.SetParent(parent);
        newObject.AddComponent<MeshFilter>();
        newObject.AddComponent<MeshRenderer>();
        newObject.AddComponent<MeshCollider>();
        return newObject;
    }

    public static void ApplyMesh(GameObject obj, Vector3[] vertices, int[] triangles, Color color)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = triangles;
        meshFilter.mesh.RecalculateNormals();
        meshRenderer.material = MaterialHandler.Instance.DefaultMaterial;
        meshRenderer.material.color = color;
        obj.AddComponent<MeshCollider>();
    }

    // Vertices in the plane have to be ordered clockwise starting from topleft corner when looking at the plane
    public static void AddPlane(ref Vector3[] vertices, ref int[] triangles, List<Vector3> planeVertices)
    {
        int startIndex = vertices.Length;

        List<Vector3> verticesNew = vertices.ToList();
        verticesNew.AddRange(planeVertices);
        vertices = verticesNew.ToArray();

        List<int> trianglesNew = triangles.ToList();
        trianglesNew.Add(startIndex);
        trianglesNew.Add(startIndex + 1);
        trianglesNew.Add(startIndex + 2);

        trianglesNew.Add(startIndex);
        trianglesNew.Add(startIndex + 2);
        trianglesNew.Add(startIndex + 3);
        triangles = trianglesNew.ToArray();
    }

    // Vertices in the cube have to be ordered [ LeftBackTop, RightBackTop, RightFrontTop, LeftFrontTop, LeftBackBot, RightBackBot, RightFrontBot, LeftFrontBot ]
    public static void AddCuboid(ref Vector3[] vertices, ref int[] triangles, Vector3 leftBackTop, Vector3 rightBackTop, Vector3 rightFrontTop, Vector3 leftFrontTop, Vector3 leftBackBot, Vector3 rightBackBot, Vector3 rightFrontBot, Vector3 leftFrontBot)
    {
        int startIndex = vertices.Length;

        // Top Side
        List<Vector3> topSide = new List<Vector3>() { leftBackTop, rightBackTop, rightFrontTop, leftFrontTop };
        AddPlane(ref vertices, ref triangles, topSide);

        // Bot Side
        List<Vector3> botSide = new List<Vector3>() { rightBackBot, leftBackBot, leftFrontBot, rightFrontBot };
        AddPlane(ref vertices, ref triangles, botSide);

        // Left Side
        List<Vector3> leftSide = new List<Vector3>() { leftBackTop, leftFrontTop, leftFrontBot, leftBackBot };
        AddPlane(ref vertices, ref triangles, leftSide);

        // Right Side
        List<Vector3> rightSide = new List<Vector3>() { rightFrontTop, rightBackTop, rightBackBot, rightFrontBot };
        AddPlane(ref vertices, ref triangles, rightSide);

        // Front Side
        List<Vector3> frontSide = new List<Vector3>() { leftFrontTop, rightFrontTop, rightFrontBot, leftFrontBot };
        AddPlane(ref vertices, ref triangles, frontSide);

        // Back Side
        List<Vector3> backSide = new List<Vector3>() { rightBackTop, leftBackTop, leftBackBot, rightBackBot };
        AddPlane(ref vertices, ref triangles, backSide);
    }

    public static void AddCube(ref Vector3[] vertices, ref int[] triangles, float left, float right, float front, float back, float top, float bot)
    {
        int startIndex = vertices.Length;

        Vector3 leftBackTop = new Vector3(left, top, back);
        Vector3 rightBackTop = new Vector3(right, top, back);
        Vector3 rightFrontTop = new Vector3(right, top, front);
        Vector3 leftFrontTop = new Vector3(left, top, front);

        Vector3 leftBackBot = new Vector3(left, bot, back);
        Vector3 rightBackBot = new Vector3(right, bot, back);
        Vector3 rightFrontBot = new Vector3(right, bot, front);
        Vector3 leftFrontBot = new Vector3(left, bot, front);

        AddCuboid(ref vertices, ref triangles, leftBackTop, rightBackTop, rightFrontTop, leftFrontTop, leftBackBot, rightBackBot, rightFrontBot, leftFrontBot);
    }
}
