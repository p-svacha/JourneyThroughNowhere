using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPathPoint
{
    public Vector3 Position;
    public List<RailPathPoint> ConnectedPoints = new List<RailPathPoint>();
    public List<RailSegment> Segments = new List<RailSegment>();
    public RailPathPoint PreviousPoint; // point on main track leading here

    public RailPathPoint(Vector3 pos, RailPathPoint prevPoint)
    {
        Position = pos;
        PreviousPoint = prevPoint;
    }

    public void AddConnection(RailPathPoint point)
    {
        ConnectedPoints.Add(point);
    }
}
