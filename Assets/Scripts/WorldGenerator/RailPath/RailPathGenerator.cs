using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPathGenerator
{
    public const float RailSegmentLength = 2f;
    private const float MaxCurveChange = 0.1f;
    private const float MaxSlopeChange = 0.1f;

    private TerrainGenerator Terrain;

    private float CurrentAngle;
    private float CurrentCurve;

    private float CurrentElevation;
    private float CurrentSlope;
    
    private List<RailPathPoint> PathPoints = new List<RailPathPoint>();
    public List<RailSegment> RailSegments = new List<RailSegment>();
    private RailPathPoint CurrentPoint;

    private RailSettings RailSettings;

    public RailPathGenerator(TerrainGenerator terrain)
    {
        Terrain = terrain;
        CurrentAngle = 0f;

        CurrentPoint = new RailPathPoint(new Vector3(0f, Terrain.GetElevation(new Vector2(0f, 0f)), 0f), null);
        PathPoints.Add(CurrentPoint);
        RailSettings = new RailSettings(plankWidth: 3f, plankHeight: 0.2f, trackWidth: 0.1f, trackGap: 1.5f, trackHeight: 0.2f);
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

        // Draw segments
        for (int i = 1; i < PathPoints.Count; i++)
        {
            RailSegment newSegment = RailSegmentMeshGenerator.GenerateRailSegment(railPath, PathPoints[i - 1], PathPoints[i], RailSettings);
            newSegment.Init(PathPoints[i - 1], PathPoints[i], RailSettings);
            RailSegments.Add(newSegment);
        }

        // Init segment connections
        foreach(RailPathPoint rpp in PathPoints)
        {
            foreach(RailSegment rs1 in rpp.Segments)
            {
                foreach(RailSegment rs2 in rpp.Segments)
                {
                    if(rs1 != rs2 && !rs1.ConnectedSegments.Contains(rs2) && !rs2.ConnectedSegments.Contains(rs1))
                    {
                        rs1.ConnectedSegments.Add(rs2);
                        rs2.ConnectedSegments.Add(rs1);
                    }
                }
            }
        }
    }

    private void GeneratePathSegment()
    {
        float nextX = CurrentPoint.Position.x + (Mathf.Sin(Mathf.Deg2Rad * CurrentAngle) * RailSegmentLength);
        float nextZ = CurrentPoint.Position.z + (Mathf.Cos(Mathf.Deg2Rad * CurrentAngle) * RailSegmentLength);

        float elevation = Terrain.GetElevation(new Vector2(nextX, nextZ));

        Vector3 newPosition = new Vector3(nextX, elevation, nextZ);
        RailPathPoint nextPoint = new RailPathPoint(newPosition, CurrentPoint);
        AddConnection(CurrentPoint, nextPoint);
        PathPoints.Add(nextPoint);
        CurrentPoint = nextPoint;

        float angleChange = Random.Range(0f, MaxCurveChange * 2f) - MaxCurveChange;
        CurrentCurve += angleChange;
        CurrentAngle += CurrentCurve;

        float elevationChange = Random.Range(0f, MaxSlopeChange * 2f) - MaxSlopeChange;
        CurrentSlope += elevationChange;
        CurrentElevation += CurrentSlope;
    }

    private void AddConnection(RailPathPoint p1, RailPathPoint p2)
    {
        p1.AddConnection(p2);
        p2.AddConnection(p1);
    }

}
