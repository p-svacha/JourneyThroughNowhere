using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshGenerator
{
    public static GameObject CreateEmptyObject(string name, Transform parent)
    {
        GameObject newObject = new GameObject(name);
        newObject.layer = LayerMask.NameToLayer("Ground");
        newObject.transform.SetParent(parent);
        newObject.AddComponent<MeshFilter>();
        newObject.AddComponent<MeshRenderer>();
        newObject.AddComponent<MeshCollider>();
        return newObject;
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
    public static void AddCube(ref Vector3[] vertices, ref int[] triangles, List<Vector3> cubeVertices)
    {
        int startIndex = vertices.Length;

        Vector3 leftBackTop = cubeVertices[0];
        Vector3 rightBackTop = cubeVertices[1];
        Vector3 rightFrontTop = cubeVertices[2];
        Vector3 leftFrontTop = cubeVertices[3];

        Vector3 leftBackBot = cubeVertices[4];
        Vector3 rightBackBot = cubeVertices[5];
        Vector3 rightFrontBot = cubeVertices[6];
        Vector3 leftFrontBot = cubeVertices[7];

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
}
