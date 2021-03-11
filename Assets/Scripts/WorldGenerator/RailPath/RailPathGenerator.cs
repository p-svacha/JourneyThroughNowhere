﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPathGenerator
{
    public const float RailSegmentLength = 2f;
    private const float MaxAngleChange = 3f;

    private float CurrentAngle;
    
    private List<RailPathPoint> PathPoints = new List<RailPathPoint>();
    public List<RailSegment> RailSegments = new List<RailSegment>();
    private RailPathPoint CurrentPoint;

    private RailSettings RailSettings;

    public RailPathGenerator()
    {
        CurrentAngle = 0f;

        CurrentPoint = new RailPathPoint(new Vector3(0f, 0f, 0f), null);
        PathPoints.Add(CurrentPoint);
        RailSettings = new RailSettings(plankWidth: 6f, plankHeight: 0.2f, trackWidth: 0.5f, trackGap: 3f, trackHeight: 0.2f);
    }

    public void GeneratePath(int numSegments)
    {
        for(int i = 0; i < numSegments; i++)
        {
            GeneratePathSegment();
        }
    }

    public void DebugPath()
    {
        for(int i = 1; i < PathPoints.Count; i++)
        {
            Debug.DrawLine(PathPoints[i - 1].Position, PathPoints[i].Position, Color.red, 20f, false);
        }
    }

    public void DrawPath()
    {
        GameObject railPath = new GameObject("RailPath");
        for (int i = 1; i < PathPoints.Count; i++)
        {
            RailSegment newSegment = RailSegmentMeshGenerator.GenerateRailSegment(railPath, PathPoints[i - 1], PathPoints[i], RailSettings);
            newSegment.Init(PathPoints[i - 1], PathPoints[i], RailSettings);
            RailSegments.Add(newSegment);
        }
    }

    private void GeneratePathSegment()
    {
        float nextX = CurrentPoint.Position.x + (Mathf.Sin(Mathf.Deg2Rad * CurrentAngle) * RailSegmentLength);
        float nextY = 0f;
        float nextZ = CurrentPoint.Position.z + (Mathf.Cos(Mathf.Deg2Rad * CurrentAngle) * RailSegmentLength);
        Vector3 newPosition = new Vector3(nextX, nextY, nextZ);
        Debug.Log(newPosition);
        RailPathPoint nextPoint = new RailPathPoint(newPosition, CurrentPoint);
        AddConnection(CurrentPoint, nextPoint);
        PathPoints.Add(nextPoint);
        CurrentPoint = nextPoint;

        float angleChange = Random.Range(0f, MaxAngleChange * 2f) - MaxAngleChange;
        CurrentAngle += angleChange;
    }

    private void AddConnection(RailPathPoint p1, RailPathPoint p2)
    {
        p1.AddConnection(p2);
        p2.AddConnection(p1);
    }

}
