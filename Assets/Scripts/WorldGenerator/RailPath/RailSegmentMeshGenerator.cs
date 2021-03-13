using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RailSegmentMeshGenerator
{
    public static RailSegment GenerateRailSegment(GameObject railPathObject, RailPathPoint p1, RailPathPoint p2, RailSettings settings)
    {
        GameObject railSegmentObject = new GameObject("RailSegment");
        railSegmentObject.transform.SetParent(railPathObject.transform);
        RailSegment railSegment = railSegmentObject.AddComponent<RailSegment>();

        Vector3 toVector = p2.Position - p1.Position;
        Vector3 toVectorPpc = (Quaternion.Euler(0, 90, 0) * new Vector3(toVector.x, 0f, toVector.z)).normalized; // PPC = perpendicular (90°)

        Vector3 fromVector = p1.PreviousPoint != null ? p1.Position - p1.PreviousPoint.Position : toVector;
        Vector3 fromVectorPpc = (Quaternion.Euler(0, 90, 0) * new Vector3(fromVector.x, 0f, fromVector.z)).normalized;

        float offsetLeft = - (settings.TrackGap / 2 + settings.TrackWidth);
        float offsetRight = settings.TrackGap / 2;

        GenerateTrack(railSegmentObject, p1, p2, settings, fromVectorPpc, toVectorPpc, offsetLeft);
        GenerateTrack(railSegmentObject, p1, p2, settings, fromVectorPpc, toVectorPpc, offsetRight);
        GeneratePlank(railSegmentObject, p1, p2, settings, toVector, toVectorPpc);

        return railSegment;
    }

    private static void GenerateTrack(GameObject segmentObject, RailPathPoint p1, RailPathPoint p2, RailSettings settings, Vector3 fromVectorPpc, Vector3 toVectorPpc, float offset)
    {
        GameObject trackObject = new GameObject("Track");
        trackObject.layer = LayerMask.NameToLayer("Ground");
        trackObject.transform.SetParent(segmentObject.transform);
        MeshFilter meshFilter = trackObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = trackObject.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[0];
        int[] triangles = new int[0];

        Vector3 heightOffsetUpper = new Vector3(0f, settings.TrackHeight + settings.PlankHeight, 0f);
        Vector3 heightOffsetLower = new Vector3(0f, settings.PlankHeight, 0f);

        // Left Side
        List<Vector3> leftSide = new List<Vector3>()
        {
            p2.Position + (toVectorPpc * offset) + heightOffsetUpper,
            p1.Position + (fromVectorPpc * offset) + heightOffsetUpper,
            p1.Position + (fromVectorPpc * offset) + heightOffsetLower,
            p2.Position + (toVectorPpc * offset) + heightOffsetLower,
        };
        MeshGenerator.AddPlane(ref vertices, ref triangles, leftSide);

        // Right Side
        List<Vector3> rightSide = new List<Vector3>()
        {
            p1.Position + (fromVectorPpc * (offset + settings.TrackWidth)) + heightOffsetUpper,
            p2.Position + (toVectorPpc * (offset + settings.TrackWidth)) + heightOffsetUpper,
            p2.Position + (toVectorPpc * (offset + settings.TrackWidth)) + heightOffsetLower,
            p1.Position + (fromVectorPpc * (offset + settings.TrackWidth)) + heightOffsetLower,
        };
        MeshGenerator.AddPlane(ref vertices, ref triangles, rightSide);

        // Top Side
        List<Vector3> topSide = new List<Vector3>()
        {
            p1.Position + (fromVectorPpc * offset) + heightOffsetUpper,
            p2.Position + (toVectorPpc * offset) + heightOffsetUpper,
            p2.Position + (toVectorPpc * (offset + settings.TrackWidth)) + heightOffsetUpper,
            p1.Position + (fromVectorPpc * (offset + settings.TrackWidth)) + heightOffsetUpper,
        };
        MeshGenerator.AddPlane(ref vertices, ref triangles, topSide);


        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = triangles;
        meshFilter.mesh.RecalculateNormals();
        meshRenderer.material = MaterialHandler.Instance.DefaultMaterial;
        meshRenderer.material.color = MaterialHandler.Instance.RailTrackColor;
        trackObject.AddComponent<MeshCollider>();
    }

    private static void GeneratePlank(GameObject segmentObject, RailPathPoint p1, RailPathPoint p2, RailSettings settings, Vector3 toVector, Vector3 toVectorPpc)
    {
        GameObject plankObject = MeshGenerator.CreateEmptyObject("Plank", "Ground", segmentObject.transform);

        Vector3[] vertices = new Vector3[0];
        int[] triangles = new int[0];

        Vector3 frontOrigin = p1.Position + toVector / 3f;
        Vector3 backOrigin = p1.Position + toVector / 3f * 2f;
        Vector3 heightOffset = new Vector3(0f, settings.PlankHeight, 0f);

        Vector3 leftBackTop = backOrigin - (toVectorPpc * settings.PlankWidth / 2f) + heightOffset;
        Vector3 rightBackTop = backOrigin + (toVectorPpc * settings.PlankWidth / 2f) + heightOffset;
        Vector3 rightFrontTop = frontOrigin + (toVectorPpc * settings.PlankWidth / 2f) + heightOffset;
        Vector3 leftFrontTop = frontOrigin - (toVectorPpc * settings.PlankWidth / 2f) + heightOffset;

        Vector3 leftBackBot = backOrigin - (toVectorPpc * settings.PlankWidth / 2f);
        Vector3 rightBackBot = backOrigin + (toVectorPpc * settings.PlankWidth / 2f);
        Vector3 rightFrontBot = frontOrigin + (toVectorPpc * settings.PlankWidth / 2f);
        Vector3 leftFrontBot = frontOrigin - (toVectorPpc * settings.PlankWidth / 2f);

        MeshGenerator.AddCuboid(ref vertices, ref triangles, leftBackTop, rightBackTop, rightFrontTop, leftFrontTop, leftBackBot, rightBackBot, rightFrontBot, leftFrontBot);
        MeshGenerator.ApplyMesh(plankObject, vertices, triangles, MaterialHandler.Instance.RailPlankColor);
    }
}
