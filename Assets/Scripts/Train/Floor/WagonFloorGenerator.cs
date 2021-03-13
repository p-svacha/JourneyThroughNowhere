using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WagonFloorGenerator
{
    public static WagonFloor GenerateWagonFloor()
    {
        GameObject floorObject = MeshGenerator.CreateEmptyObject("Floor", "Train");

        Vector3[] vertices = new Vector3[0];
        int[] triangles = new int[0];

        float left = -Wagon.Width / 2;
        float right = Wagon.Width / 2;
        float front = 0f;
        float back = Wagon.Length;
        float bot = 0f;
        float top = WagonFloor.FloorHeight;

        MeshGenerator.AddCube(ref vertices, ref triangles, left, right, front, back, top, bot);
        MeshGenerator.ApplyMesh(floorObject, vertices, triangles, MaterialHandler.Instance.DefaultColor);

        WagonFloor floor = floorObject.AddComponent<WagonFloor>();
        floor.Init(null);

        return floor;
    }
}
